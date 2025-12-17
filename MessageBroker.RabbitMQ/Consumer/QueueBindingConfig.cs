namespace MessageBroker.RabbitMQ.Consumer;

public class QueueBindingConfig
{
    public string Queue { get; init; } = string.Empty;
    public string Exchange { get; init; } = string.Empty;
    public string RoutingKey { get; init; } = string.Empty;
    public string ExchangeType { get; init; } = "topic";
    
    
    public bool Durable { get; init; } = true;
    public bool Exclusive { get; init; } = false;
    public bool AutoDelete { get; init; } = false;
    public IDictionary<string, object>? Arguments { get; init; }
}