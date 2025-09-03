
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.Messaging;

public sealed class RabbitPublisher : IAsyncDisposable
{
    private readonly string _url;
    private readonly string _queue;
    private IConnection? _conn;
    private IChannel? _ch;

    public RabbitPublisher(IConfiguration cfg)
    {
        _url   = cfg["RABBITMQ_URL"]  ?? "amqp://guest:guest@rabbitmq:5672/";
        _queue = cfg["RABBITMQ_QUEUE"] ?? "events";
    }

    private async Task EnsureAsync()
    {
        if (_ch != null) return;
        var factory = new ConnectionFactory { Uri = new Uri(_url) };
        _conn = await factory.CreateConnectionAsync();
        _ch   = await _conn.CreateChannelAsync();
        await _ch.QueueDeclareAsync(_queue, durable: true, exclusive: false, autoDelete: false);
    }

    public async Task PublishAsync(object payload)
    {
        await EnsureAsync();
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
        await _ch!.BasicPublishAsync(exchange: "", routingKey: _queue, body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_ch   != null) await _ch.CloseAsync();
        if (_conn != null) await _conn.CloseAsync();
    }
}