using MessageBroker.Abstractions.Interfaces;

namespace MessageBroker.RabbitMQ.Consumer;

public class ConsumerRoutingBuilder
{
    private readonly Dictionary<Type, QueueBindingConfig> _bindings = new();
    private string? _defaultQueuePrefix;

    public ConsumerRoutingBuilder WithDefaultQueuePrefix(string prefix)
    {
        _defaultQueuePrefix = prefix;
        return this;
    }

    public ConsumerRoutingBuilder Bind<TEvent> (
        string queueName,
        string exchange,
        string routingKey,
        bool durable = true,
        IDictionary<string, object>? arguments = null) where TEvent : IDomainEvent
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

    public ConsumerRoutingBuilder BindRange(Action<ConsumerRoutingBuilder> configure)
    {
        configure(this);
        return this;
    }

    internal IReadOnlyDictionary<Type, QueueBindingConfig> Build() => _bindings;
}