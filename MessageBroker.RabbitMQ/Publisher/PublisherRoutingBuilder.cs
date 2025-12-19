using MessageBroker.Abstractions.Interfaces;

namespace MessageBroker.RabbitMQ.Publisher;

/// <summary>
/// Построитель маршрутизации для настройки привязок событий к обменникам при публикации сообщений.
/// </summary>
public class PublisherRoutingBuilder
{
    private readonly Dictionary<Type, EventRoutingConfig> _routes = new();
    private string? _defaultExchangePrefix;

    /// <summary>
    /// Устанавливает префикс по умолчанию для всех обменников.
    /// </summary>
    /// <param name="prefix">Префикс, который будет добавлен к именам всех обменников.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public PublisherRoutingBuilder WithDefaultExchangePrefix(string prefix)
    {
        _defaultExchangePrefix = prefix;
        return this;
    }

    /// <summary>
    /// Сопоставляет тип события с обменником и ключом маршрутизации для публикации.
    /// </summary>
    /// <typeparam name="TEvent">Тип доменного события, реализующий <see cref="IDomainEvent"/>.</typeparam>
    /// <param name="exchange">Имя обменника RabbitMQ для публикации события.</param>
    /// <param name="routingKey">Ключ маршрутизации для публикации события.</param>
    /// <param name="exchangeType">Тип обменника (topic, direct, fanout, headers). По умолчанию: "topic".</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public PublisherRoutingBuilder Map<TEvent>(
        string exchange, 
        string routingKey, 
        string exchangeType = "topic") where TEvent : IIntegrationEvent
    {
        var finalExchange = _defaultExchangePrefix != null 
            ? $"{_defaultExchangePrefix}.{exchange}" 
            : exchange;

        _routes[typeof(TEvent)] = new EventRoutingConfig
        {
            Exchange = finalExchange,
            RoutingKey = routingKey,
            ExchangeType = exchangeType
        };
        return this;
    }

    /// <summary>
    /// Позволяет настроить несколько сопоставлений через делегат для удобства группировки конфигурации.
    /// </summary>
    /// <param name="configure">Действие для настройки нескольких сопоставлений.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public PublisherRoutingBuilder MapRange(Action<PublisherRoutingBuilder> configure)
    {
        configure(this);
        return this;
    }

    internal IReadOnlyDictionary<Type, EventRoutingConfig> Build() => _routes;
}