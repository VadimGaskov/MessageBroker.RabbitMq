using MessageBroker.Abstractions.Interfaces;
using MessageBroker.RabbitMQ.Consumer;
using MessageBroker.RabbitMQ.Publisher;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ.Registration;

/// <summary>
/// Регистратор публикаторов сообщений RabbitMQ в DI-контейнере.
/// </summary>
public static class ProducerRegistrar
{
    /// <summary>
    /// Регистрирует публикатор сообщений и маршрутизацию событий в коллекции сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="configure">Действие для настройки маршрутизации публикации событий.</param>
    public static void Register(IServiceCollection services, Action<PublisherRoutingBuilder> configure)
    {
        // Построение маршрутов
        var builder = new PublisherRoutingBuilder();
        
        configure(builder);
        
        var routes = builder.Build();

        // Регистрация маршрутов как singleton
        services.AddSingleton(routes);

        // Регистрация publisher как singleton
        services.AddSingleton<IMessageBrokerPublisher, RabbitMqPublisher>();
    }
}