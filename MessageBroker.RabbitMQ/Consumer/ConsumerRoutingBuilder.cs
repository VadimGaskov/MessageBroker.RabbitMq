using MessageBroker.Abstractions.Interfaces;

namespace MessageBroker.RabbitMQ.Consumer;

/// <summary>
/// Построитель маршрутизации для настройки привязок очередей к событиям при потреблении сообщений.
/// </summary>
public class ConsumerRoutingBuilder
{
    private readonly Dictionary<Type, QueueBindingConfig> _bindings = new();
    private string? _defaultQueuePrefix;

    /// <summary>
    /// Устанавливает префикс по умолчанию для всех очередей.
    /// </summary>
    /// <param name="prefix">Префикс, который будет добавлен к именам всех очередей.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public ConsumerRoutingBuilder WithDefaultQueuePrefix(string prefix)
    {
        _defaultQueuePrefix = prefix;
        return this;
    }

    /// <summary>
    /// Привязывает тип события к очереди RabbitMQ с указанными параметрами маршрутизации.
    /// </summary>
    /// <typeparam name="TEvent">Тип доменного события, реализующий <see cref="IDomainEvent"/>.</typeparam>
    /// <param name="queueName">Имя очереди для привязки.</param>
    /// <param name="exchange">Имя обменника RabbitMQ.</param>
    /// <param name="routingKey">Ключ маршрутизации для привязки очереди к обменнику.</param>
    /// <param name="durable">Указывает, должна ли очередь быть устойчивой (сохраняться после перезапуска сервера).</param>
    /// <param name="arguments">Дополнительные аргументы для привязки очереди.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public ConsumerRoutingBuilder Bind<TEvent> (
        string queueName,
        string exchange,
        string routingKey,
        bool durable = true,
        IDictionary<string, object>? arguments = null) where TEvent : IIntegrationEvent
    {
        var finalQueueName = _defaultQueuePrefix != null
            ? $"{_defaultQueuePrefix}.{queueName}"
            : queueName;

        _bindings[typeof(TEvent)] = new QueueBindingConfig
        {
            Queue = finalQueueName,
            Exchange = exchange,
            RoutingKey = routingKey,
            Durable = durable,
            Arguments = arguments
        };
        return this;
    }

    /// <summary>
    /// Позволяет настроить несколько привязок через делегат для удобства группировки конфигурации.
    /// </summary>
    /// <param name="configure">Действие для настройки нескольких привязок.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public ConsumerRoutingBuilder BindRange(Action<ConsumerRoutingBuilder> configure)
    {
        configure(this);
        return this;
    }

    internal IReadOnlyDictionary<Type, QueueBindingConfig> Build() => _bindings;
}