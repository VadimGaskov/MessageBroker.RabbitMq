using MessageBroker.Abstractions.Interfaces;
using MessageBroker.RabbitMQ.Consumer;
using MessageBroker.RabbitMQ.Publisher;
using MessageBroker.RabbitMQ.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ;

public class RabbitMqServiceCollectionBuilder
{
    private Action<PublisherRoutingBuilder>? _producerRoutingBuilder;
    private Action<ConsumerRoutingBuilder>? _consumerRoutingBuilder;
    private Action<RabbitMqSettings>? _rabbitMqSettings;
    private bool _settingsFromConfig = false;

    public RabbitMqServiceCollectionBuilder ConfigureProducer(Action<PublisherRoutingBuilder> configure)
    {
        _producerRoutingBuilder = configure;
        
        return this;
    }

    public RabbitMqServiceCollectionBuilder ConfigureConsumer(Action<ConsumerRoutingBuilder> configure)
    {
        _consumerRoutingBuilder = configure;
        
        return this;
    }

    public RabbitMqServiceCollectionBuilder ConfigureSettings(Action<RabbitMqSettings> configure)
    {
        _rabbitMqSettings = configure;
        return this;
    }

    internal RabbitMqServiceCollectionBuilder UseSettingsFromConfiguration()
    {
        _settingsFromConfig = true;
        
        return this;
    }
    
    internal void Build(IServiceCollection services)
    {
        if (!_settingsFromConfig && _rabbitMqSettings == null)
            throw new InvalidOperationException("RabbitMQ settings must be provided via configuration section or ConfigureSettings.");

        
        if (_rabbitMqSettings != null && !_settingsFromConfig)
            services.Configure<RabbitMqSettings>(_rabbitMqSettings);
        
        ConnectionRegistrar.Register(services);

        services.AddSingleton<RabbitMqConnectionManager>();
        
        if (_producerRoutingBuilder != null)
            ProducerRegistrar.Register(services, _producerRoutingBuilder);

        if (_consumerRoutingBuilder != null)
            ConsumerRegistrar.Register(services, _consumerRoutingBuilder);
    }
    
}