using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OrderConsumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var facatory = new ConnectionFactory() { HostName = "localhost" };
            var connection = facatory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("order-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"📩 you have a new order: {message}");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume("order-queue", autoAck: false, consumer);

            Console.WriteLine("🚀 waiting msg, press enter to exist:");
            Console.ReadLine();
        }
    }
}
