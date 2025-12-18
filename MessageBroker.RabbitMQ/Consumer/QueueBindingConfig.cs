namespace MessageBroker.RabbitMQ.Consumer;

/// <summary>
/// Конфигурация привязки очереди к обменнику RabbitMQ.
/// </summary>
public class QueueBindingConfig
{
    /// <summary>
    /// Имя очереди.
    /// </summary>
    public string Queue { get; init; } = string.Empty;
    
    /// <summary>
    /// Имя обменника RabbitMQ.
    /// </summary>
    public string Exchange { get; init; } = string.Empty;
    
    /// <summary>
    /// Ключ маршрутизации для привязки очереди к обменнику.
    /// </summary>
    public string RoutingKey { get; init; } = string.Empty;
    
    /// <summary>
    /// Тип обменника (topic, direct, fanout, headers). По умолчанию: "topic".
    /// </summary>
    public string ExchangeType { get; init; } = "topic";
    
    /// <summary>
    /// Указывает, должна ли очередь быть устойчивой (сохраняться после перезапуска сервера).
    /// По умолчанию: true.
    /// </summary>
    public bool Durable { get; init; } = true;
    
    /// <summary>
    /// Указывает, должна ли очередь быть эксклюзивной (доступна только одному подключению).
    /// По умолчанию: false.
    /// </summary>
    public bool Exclusive { get; init; } = false;
    
    /// <summary>
    /// Указывает, должна ли очередь автоматически удаляться при отсутствии потребителей.
    /// По умолчанию: false.
    /// </summary>
    public bool AutoDelete { get; init; } = false;
    
    /// <summary>
    /// Дополнительные аргументы для привязки очереди (например, TTL, максимальная длина и т.д.).
    /// </summary>
    public IDictionary<string, object>? Arguments { get; init; }
}