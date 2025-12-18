using MessageBroker.RabbitMQ.Consumer;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ.Registration;

/// <summary>
/// Регистратор потребителей сообщений RabbitMQ в DI-контейнере.
/// </summary>
internal static class ConsumerRegistrar
{
    /// <summary>
    /// Регистрирует сервис потребителя сообщений и маршрутизацию событий в коллекции сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="configure">Действие для настройки маршрутизации потребления событий.</param>
    public static void Register(IServiceCollection services, Action<ConsumerRoutingBuilder> configure)
    {
        var builder = new ConsumerRoutingBuilder();

        configure(builder);

        var routes = builder.Build();

        services.AddSingleton(routes);

        services.AddHostedService<RabbitMqConsumerService>();
    }
}