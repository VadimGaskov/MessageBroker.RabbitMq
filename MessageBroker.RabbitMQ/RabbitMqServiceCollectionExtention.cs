using MessageBroker.RabbitMQ.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMQ;

public static class RabbitMqServiceCollectionExtention 
{
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