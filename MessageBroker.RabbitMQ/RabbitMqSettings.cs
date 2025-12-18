namespace MessageBroker.RabbitMQ;

/// <summary>
/// Настройки подключения к RabbitMQ.
/// </summary>
public class RabbitMqSettings
{
    /// <summary>
    /// Имя хоста или IP-адрес сервера RabbitMQ.
    /// </summary>
    public string HostName { get; set; }
    
    /// <summary>
    /// Имя пользователя для подключения к RabbitMQ.
    /// </summary>
    public string UserName { get; set; }
    
    /// <summary>
    /// Пароль для подключения к RabbitMQ.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Порт для подключения к RabbitMQ (по умолчанию 5672).
    /// </summary>
    public int Port { get; set; }
    
    /// <summary>
    /// Виртуальный хост RabbitMQ (по умолчанию "/").
    /// </summary>
    public string VirtualHost { get; set; }
}