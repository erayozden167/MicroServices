//using RabbitMQ.Client.Events;
//using RabbitMQ.Client;
//using System.Text;
//using PurchaseApi.Messages.Model;
//using System.Text.Json;

//public class RabbitMQConsumer
//{
//    private readonly IConnection _connection;
//    private readonly IModel _channel;

//    public RabbitMQConsumer()
//    {
//        var factory = new ConnectionFactory() { HostName = "localhost" }; // RabbitMQ sunucu ayarları
//        _connection = factory.CreateConnection();
//        _channel = _connection.CreateModel();

//        // Stok kontrolü yanıtı için bir kuyruk oluştur
//        _channel.QueueDeclare(queue: "stock_check_response_queue",
//                             durable: false,
//                             exclusive: false,
//                             autoDelete: false,
//                             arguments: null);
//    }

//    public async Task<bool> ConsumeStockCheckResponseAsync(int productId)
//    {
//        var tcs = new TaskCompletionSource<bool>();

//        // Mesajı dinle
//        var consumer = new EventingBasicConsumer(_channel);
//        consumer.Received += (model, ea) =>
//        {
//            var body = ea.Body.ToArray();
//            var message = Encoding.UTF8.GetString(body);

//            // JSON deserialization
//            var response = JsonSerializer.Deserialize<StockCheckResponse>(message);

//            // Ürün ID'sine göre stok kontrolü sonucu
//            if (response.ProductId == productId)
//            {
//                tcs.SetResult(response.IsAvailable);
//            }
//        };

//        _channel.BasicConsume(queue: "stock_check_response_queue",
//                             autoAck: true,
//                             consumer: consumer);

//        // Sonucu bekle
//        return await tcs.Task;
//    }

//}
