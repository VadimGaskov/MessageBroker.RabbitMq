using MessageBroker.Abstractions.Interfaces;
using MessageBroker.RabbitMQ.Consumer;
using MessageBroker.RabbitMQ.Publisher;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ.Registration;

public static class ProducerRegistrar
{
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