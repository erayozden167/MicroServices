using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQPublisher()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" }; // RabbitMQ sunucu ayarları
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Stok değişimi için bir kuyruk ..
        _channel.QueueDeclare(queue: "purchase_queue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void PublishStockUpdateMessage(int productId, int quantity)
    {
        var message = new { ProductId = productId, Quantity = quantity };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        // Mesajı kuyruğa gönder ..:
        _channel.BasicPublish(exchange: "",
                             routingKey: "purchase_queue",
                             basicProperties: null,
                             body: body);
    }
}
