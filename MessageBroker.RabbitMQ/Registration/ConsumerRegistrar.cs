using MessageBroker.RabbitMQ.Consumer;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ.Registration;

internal static class ConsumerRegistrar
{
    public static void Register(IServiceCollection services, Action<ConsumerRoutingBuilder> configure)
    {
        var builder = new ConsumerRoutingBuilder();

        configure(builder);

        var routes = builder.Build();

        services.AddSingleton(routes);

        services.AddHostedService<RabbitMqConsumerService>();
    }
}