namespace MessageBroker.RabbitMQ.Consumer;

public class RabbitMqConsumerSettings
{
    public ushort PrefetchCount { get; set; } = 10;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public bool AutoAck { get; set; } = false; // ВСЕГДА false для надёжности
}