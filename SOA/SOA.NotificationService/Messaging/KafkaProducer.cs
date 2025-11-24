using Confluent.Kafka;

namespace SOA.NotificationService.Messaging;

public class KafkaProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducer(IConfiguration configuration)
    {
        var bootstrap = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        _topic = configuration["Kafka:Topic"] ?? "grades-stream";
        _producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = bootstrap }).Build();
    }

    public Task ProduceAsync(string key, string value, CancellationToken cancellationToken = default) =>
        _producer.ProduceAsync(_topic, new Message<string, string> { Key = key, Value = value }, cancellationToken);

    public void Dispose() => _producer.Dispose();
}
