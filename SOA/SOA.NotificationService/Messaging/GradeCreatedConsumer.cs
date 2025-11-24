using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SOA.Domain.Events;
using SOA.NotificationService.Hubs;
using System.Text;
using System.Text.Json;

namespace SOA.NotificationService.Messaging;

public class GradeCreatedConsumer : BackgroundService
{
    private readonly ILogger<GradeCreatedConsumer> _logger;
    private readonly KafkaProducer _kafkaProducer;
    private readonly HttpClient _httpClient;
    private IConnection? _connection;
    private IModel? _channel;
    private GpaSseHub _gpaSseHub;

    public GradeCreatedConsumer(
        ILogger<GradeCreatedConsumer> logger,
        KafkaProducer kafkaProducer,
        IHttpClientFactory httpClientFactory,
        GpaSseHub gpaSseHub
    )
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _httpClient = httpClientFactory.CreateClient("faas");
        _gpaSseHub = gpaSseHub;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBIT_HOST") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("RABBIT_PORT"), out var port) ? port : 5672,
            UserName = Environment.GetEnvironmentVariable("RABBIT_USER") ?? "soa",
            Password = Environment.GetEnvironmentVariable("RABBIT_PASS") ?? "soa"
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.QueueDeclare("grades", durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, eventArgs) =>
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var message = JsonSerializer.Deserialize<GradeCreatedGpaEvent>(json);

            _logger.LogInformation("Grade created: {StudentId} {Course}={Value}",  message?.StudentId, message?.Course, message?.GivenGrade);

            await _kafkaProducer.ProduceAsync((message?.StudentId ?? Guid.Empty).ToString(), json, cancellationToken);

            try
            {
                var response = await _httpClient.PostAsync(
                    "/api/compute-gpa",
                    new StringContent(json, Encoding.UTF8, "application/json"),
                    cancellationToken
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("FaaS returned {Status}", response.StatusCode);
                }
                else
                {
                    var bodyText = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogInformation("FaaS OK: {Body}", bodyText);

                    double gpa = JsonSerializer.Deserialize<JsonElement>(bodyText).GetProperty("gpa").GetDouble();

                    var payload = JsonSerializer.Serialize(new
                    {
                        studentId = message?.StudentId,
                        course = message?.Course,
                        gpa
                    });

                    await _gpaSseHub.BroadcastAsync(payload, cancellationToken);

                    _logger.LogInformation("Broadcasted GPA via SSE: {Payload}", payload);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error calling FaaS");
            }
        };

        _channel.BasicConsume(queue: "grades", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();

        base.Dispose();
    }
}
