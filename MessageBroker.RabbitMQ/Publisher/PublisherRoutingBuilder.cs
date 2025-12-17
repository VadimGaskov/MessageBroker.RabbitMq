using MessageBroker.Abstractions.Interfaces;

namespace MessageBroker.RabbitMQ.Publisher;

public class PublisherRoutingBuilder
{
    private readonly Dictionary<Type, EventRoutingConfig> _routes = new();
    private string? _defaultExchangePrefix;

    public PublisherRoutingBuilder WithDefaultExchangePrefix(string prefix)
    {
        _defaultExchangePrefix = prefix;
        return this;
    }

    public PublisherRoutingBuilder Map<TEvent>(
        string exchange, 
        string routingKey, 
        string exchangeType = "topic") where TEvent : IDomainEvent
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

    public PublisherRoutingBuilder MapRange(Action<PublisherRoutingBuilder> configure)
    {
        configure(this);
        return this;
    }

    internal IReadOnlyDictionary<Type, EventRoutingConfig> Build() => _routes;
}