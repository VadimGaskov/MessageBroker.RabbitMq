using System.Text;
using System.Text.Json;
using MessageBroker.Abstractions.Interfaces;
using MessageBroker.RabbitMQ.Publisher;
using RabbitMQ.Client;

namespace MessageBroker.RabbitMQ;

public class RabbitMqPublisher : IMessageBrokerPublisher
{
    private readonly RabbitMqConnectionManager _rabbitMqConnectionManager;
    private readonly IReadOnlyDictionary<Type, EventRoutingConfig> _routes;
    
    public RabbitMqPublisher(IReadOnlyDictionary<Type, EventRoutingConfig> routes, RabbitMqConnectionManager rabbitMqConnectionManager)
    {
        _routes = routes;
        _rabbitMqConnectionManager = rabbitMqConnectionManager;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IDomainEvent
    {
        var eventType = typeof(TEvent);
        
        if (!_routes.TryGetValue(eventType, out var config))
        {
            throw new InvalidOperationException(
                $"Событие {eventType.Name} не зарегистрировано в маршрутизации. " +
                $"Добавьте его через EventRoutingBuilder.Map<{eventType.Name}>()");
        }

        var connection = await _rabbitMqConnectionManager.GetConnectionAsync(cancellationToken);
        
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await channel.ExchangeDeclareAsync(
            exchange: config.Exchange,
            type: config.ExchangeType,
            durable: true,
            autoDelete: false, 
            cancellationToken: cancellationToken);

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);
        
        var props = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true
        };
        
        await channel.BasicPublishAsync(
            exchange: config.Exchange,
            routingKey: config.RoutingKey,
            body: body,
            mandatory: false,
            basicProperties: props,
            cancellationToken: cancellationToken);
    }
}