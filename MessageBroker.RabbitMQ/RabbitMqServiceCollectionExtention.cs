using MessageBroker.RabbitMQ.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ;

/// <summary>
/// Методы расширения для регистрации сервисов RabbitMQ в DI-контейнере.
/// </summary>
public static class RabbitMqServiceCollectionExtention 
{
    /// <summary>
    /// Добавляет сервисы RabbitMQ в коллекцию сервисов с программной настройкой параметров подключения.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="configure">Действие для настройки RabbitMQ (producer, consumer, settings).</param>
    /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="configure"/> равен null.</exception>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqServiceCollectionBuilder> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        
        var rabbitMqServiceCollectionBuilder = new RabbitMqServiceCollectionBuilder();

        configure(rabbitMqServiceCollectionBuilder);
        
        rabbitMqServiceCollectionBuilder.Build(services);
        
        return services;
    }

    /// <summary>
    /// Добавляет сервисы RabbitMQ в коллекцию сервисов с настройкой параметров подключения из конфигурации.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="configurationSection">Секция конфигурации с параметрами подключения к RabbitMQ.</param>
    /// <param name="configure">Действие для настройки RabbitMQ (producer, consumer).</param>
    /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="configurationSection"/> или <paramref name="configure"/> равны null.</exception>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfigurationSection configurationSection,
        Action<RabbitMqServiceCollectionBuilder> configure)
    {
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        
        services.Configure<RabbitMqSettings>(configurationSection);
        
        var rabbitMqServiceCollectionBuilder = new RabbitMqServiceCollectionBuilder();
        
        rabbitMqServiceCollectionBuilder.UseSettingsFromConfiguration();
        
        configure(rabbitMqServiceCollectionBuilder);
        
        rabbitMqServiceCollectionBuilder.Build(services);
        
        return services;
    }
}