using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMqOrderDemos.Services
{
    public class MessagePublisher
    {
        private readonly string _hostName = "localhost";
        private readonly string _queueName = "order-queue";


        public void Publish(object message)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();


            //prepare the msg
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);


            channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            channel.BasicPublish(exchange: "", _queueName, mandatory: false, basicProperties: null, body);

        }
    }
}
