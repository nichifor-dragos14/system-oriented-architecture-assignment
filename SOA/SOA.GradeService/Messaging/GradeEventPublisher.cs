using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SOA.Domain.Events;

namespace SOA.GradeService.Messaging;

public class GradeEventPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public GradeEventPublisher(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["Rabbit:Host"] ?? "localhost",
            Port = int.TryParse(configuration["Rabbit:Port"], out var port) ? port : 5672,
            UserName = configuration["Rabbit:User"] ?? "soa",
            Password = configuration["Rabbit:Pass"] ?? "soa"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("grades", durable: false, exclusive: false, autoDelete: false);
    }

    public void PublishGradeCreated(GradeCreatedGpaEvent gradeGpaEvent)
    {
        var payload = JsonSerializer.Serialize(gradeGpaEvent);

        var body = Encoding.UTF8.GetBytes(payload);
        var properties = _channel.CreateBasicProperties();

        properties.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: "grades",
            basicProperties: properties,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
