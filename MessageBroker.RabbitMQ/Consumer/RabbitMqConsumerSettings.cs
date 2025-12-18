namespace MessageBroker.RabbitMQ.Consumer;

/// <summary>
/// Настройки для потребителя сообщений RabbitMQ.
/// </summary>
public class RabbitMqConsumerSettings
{
    /// <summary>
    /// Максимальное количество неподтверждённых сообщений, которые могут быть получены одним потребителем одновременно.
    /// По умолчанию: 10.
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;
    
    /// <summary>
    /// Максимальное количество повторных попыток обработки сообщения при ошибке.
    /// По умолчанию: 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Задержка между повторными попытками в миллисекундах.
    /// По умолчанию: 1000.
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;
    
    /// <summary>
    /// Автоматическое подтверждение получения сообщений. Всегда должно быть false для надёжности.
    /// По умолчанию: false.
    /// </summary>
    public bool AutoAck { get; set; } = false;
}