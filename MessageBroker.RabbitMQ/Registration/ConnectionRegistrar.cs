using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageBroker.RabbitMQ.Registration;

/// <summary>
/// Регистратор подключений к RabbitMQ в DI-контейнере.
/// </summary>
public static class ConnectionRegistrar
{
    /// <summary>
    /// Регистрирует фабрику подключений к RabbitMQ в коллекции сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
            
            return new ConnectionFactory
            {
                HostName = rabbitMqSettings.HostName,
                Port = rabbitMqSettings.Port,
                UserName = rabbitMqSettings.UserName,
                Password = rabbitMqSettings.Password,
                VirtualHost = rabbitMqSettings.VirtualHost ?? "/",

                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
            };

            /*
            var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            
            // ✅ КРИТИЧНО: регистрируем освобождение ресурсов
            var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    connection.CloseAsync(); // Корректное закрытие протокола AMQP
                    connection.Dispose(); // Освобождение native ресурсов
                }
                catch (Exception ex)
                {
                    // Логируем, но не пробрасываем - shutdown не должен падать
                    // sp.GetRequiredService<ILogger<...>>().LogError(ex, "Error closing RabbitMQ connection");
                }
            });

            return connection;*/
        });
    }
}