using RabbitMQ.Client;

namespace MessageBroker.RabbitMQ;

/// <summary>
/// Управляет подключением к RabbitMQ с поддержкой потокобезопасного создания и переиспользования соединения.
/// </summary>
public class RabbitMqConnectionManager : IAsyncDisposable
{
    private readonly IConnectionFactory _factory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="RabbitMqConnectionManager"/>.
    /// </summary>
    /// <param name="factory">Фабрика для создания подключений к RabbitMQ.</param>
    public RabbitMqConnectionManager(IConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Получает активное подключение к RabbitMQ. Если подключение отсутствует или закрыто, создаёт новое.
    /// </summary>
    /// <param name="ct">Токен отмены для асинхронной операции.</param>
    /// <returns>Активное подключение к RabbitMQ.</returns>
    public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _lock.WaitAsync(ct);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            _connection = await _factory.CreateConnectionAsync(ct);
            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Асинхронно освобождает ресурсы, закрывая подключение к RabbitMQ.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}