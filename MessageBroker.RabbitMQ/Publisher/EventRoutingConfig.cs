namespace MessageBroker.RabbitMQ.Publisher;

public class EventRoutingConfig
{
    public string Exchange { get; init; } = string.Empty;
    public string RoutingKey { get; init; } = string.Empty;
    public string ExchangeType { get; init; } = "topic";
}