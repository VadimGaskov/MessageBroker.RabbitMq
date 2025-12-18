using MessageBroker.Abstractions.Interfaces;
using MessageBroker.RabbitMQ.Consumer;
using MessageBroker.RabbitMQ.Publisher;
using MessageBroker.RabbitMQ.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ;

/// <summary>
/// Построитель конфигурации сервисов RabbitMQ для регистрации в DI-контейнере.
/// </summary>
public class RabbitMqServiceCollectionBuilder
{
    private Action<PublisherRoutingBuilder>? _producerRoutingBuilder;
    private Action<ConsumerRoutingBuilder>? _consumerRoutingBuilder;
    private Action<RabbitMqSettings>? _rabbitMqSettings;
    private bool _settingsFromConfig = false;

    /// <summary>
    /// Настраивает маршрутизацию для публикации сообщений (producer).
    /// </summary>
    /// <param name="configure">Действие для настройки маршрутизации публикации событий.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public RabbitMqServiceCollectionBuilder ConfigureProducer(Action<PublisherRoutingBuilder> configure)
    {
        _producerRoutingBuilder = configure;
        
        return this;
    }

    /// <summary>
    /// Настраивает маршрутизацию для потребления сообщений (consumer).
    /// </summary>
    /// <param name="configure">Действие для настройки маршрутизации потребления событий.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public RabbitMqServiceCollectionBuilder ConfigureConsumer(Action<ConsumerRoutingBuilder> configure)
    {
        _consumerRoutingBuilder = configure;
        
        return this;
    }

    /// <summary>
    /// Настраивает параметры подключения к RabbitMQ программно.
    /// </summary>
    /// <param name="configure">Действие для настройки параметров подключения.</param>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    public RabbitMqServiceCollectionBuilder ConfigureSettings(Action<RabbitMqSettings> configure)
    {
        _rabbitMqSettings = configure;
        return this;
    }

    /// <summary>
    /// Указывает, что настройки подключения будут загружены из конфигурации.
    /// </summary>
    /// <returns>Экземпляр построителя для цепочки вызовов.</returns>
    internal RabbitMqServiceCollectionBuilder UseSettingsFromConfiguration()
    {
        _settingsFromConfig = true;
        
        return this;
    }
    
    /// <summary>
    /// Выполняет регистрацию всех настроенных сервисов RabbitMQ в DI-контейнере.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <exception cref="InvalidOperationException">Выбрасывается, если настройки подключения не предоставлены.</exception>
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