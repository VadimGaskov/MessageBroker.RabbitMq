namespace MessageBroker.RabbitMQ.Publisher;

/// <summary>
/// Конфигурация маршрутизации события для публикации в RabbitMQ.
/// </summary>
public class EventRoutingConfig
{
    /// <summary>
    /// Имя обменника RabbitMQ для публикации события.
    /// </summary>
    public string Exchange { get; init; } = string.Empty;
    
    /// <summary>
    /// Ключ маршрутизации для публикации события.
    /// </summary>
    public string RoutingKey { get; init; } = string.Empty;
    
    /// <summary>
    /// Тип обменника (topic, direct, fanout, headers). По умолчанию: "topic".
    /// </summary>
    public string ExchangeType { get; init; } = "topic";
}