using System.Text;
using System.Text.Json;
using MessageBroker.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBroker.RabbitMQ.Consumer;

//TODO Внедрить RabbitMqConsumerSettings
public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyDictionary<Type, QueueBindingConfig> _routes;
    private readonly List<IChannel> _channels = new();
    private readonly RabbitMqConsumerSettings _rabbitMqConsumerSettings;
    private readonly RabbitMqConnectionManager _rabbitMqConnectionManager;
    
    public RabbitMqConsumerService(IServiceProvider serviceProvider, IReadOnlyDictionary<Type, 
        QueueBindingConfig> routes, IOptions<RabbitMqConsumerSettings> rabbitMqConsumerSettings, RabbitMqConnectionManager rabbitMqConnectionManager)
    {
        _serviceProvider = serviceProvider;
        _routes = routes;
        _rabbitMqConnectionManager = rabbitMqConnectionManager;
        _rabbitMqConsumerSettings = rabbitMqConsumerSettings.Value;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var connection = await _rabbitMqConnectionManager.GetConnectionAsync(cancellationToken);
        
        foreach (var route in _routes)
        {
            var eventType = route.Key;
            var config = route.Value;

            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            _channels.Add(channel);
            
            await channel.BasicQosAsync(
                prefetchSize: 0, 
                prefetchCount: _rabbitMqConsumerSettings.PrefetchCount, 
                global: false, 
                cancellationToken: cancellationToken);
            
            await channel.ExchangeDeclareAsync(
                exchange: config.Exchange, 
                type: config.ExchangeType, 
                durable: config.Durable,
                cancellationToken: cancellationToken);
            
            await channel.QueueDeclareAsync(
                queue: config.Queue, 
                durable: config.Durable, 
                exclusive: config.Exclusive, 
                autoDelete: config.AutoDelete, 
                cancellationToken: cancellationToken);
            
            await channel.QueueBindAsync(
                queue: config.Queue, 
                exchange: config.Exchange, 
                routingKey: config.RoutingKey, 
                cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            
            consumer.ReceivedAsync += (model, ea) => HandleMessage(eventType, ea, channel, cancellationToken); 
            
            await channel.BasicConsumeAsync(
                queue: config.Queue, 
                autoAck: false, consumer, 
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleMessage(Type type, BasicDeliverEventArgs ea, IChannel channel, CancellationToken cancellationToken)
    {
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
        var @event = JsonSerializer.Deserialize(json, type);
        
        if (@event == null)
        {
            // Логируем и делаем Nack с requeue: false (отправляем в DLQ)
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken);
            return;
        }
        
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var handlerType = typeof(IEventHandler<>).MakeGenericType(type);
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            var method = handlerType.GetMethod("HandleAsync");

            await (Task)method.Invoke(handler, new[] { @event, cancellationToken });

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
        }
        catch (Exception e)
        {
            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken);
            //TODO Заменить на логирование чтобы сервис не падал и можно было дебажить!!!
            Console.WriteLine($"no handler type is registered for ${nameof(type)}");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var channel in _channels)
        {
            try
            {
                await channel.CloseAsync(cancellationToken);
                channel.Dispose();
            }
            catch (Exception ex)
            {
                // Логируем, но продолжаем закрывать остальные
            }
        }
        
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        // Channels уже закрыты в StopAsync, но на всякий случай:
        foreach (var channel in _channels)
        {
            try
            {
                channel.Dispose();
            }
            catch
            {
                // Игнорируем, т.к. уже в процессе очистки
            }
        }
    
        base.Dispose();
    }
}