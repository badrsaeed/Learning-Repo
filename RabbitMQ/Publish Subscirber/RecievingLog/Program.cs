using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

IConnectionFactory factory = new ConnectionFactory();
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

// declare a server-named queue
var queuName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue:queuName, exchange:"logs", routingKey:string.Empty);

Console.WriteLine(" [*] Waiting for logs.");

EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray() ;
    string msg = Encoding.UTF8.GetString(body) ;

    Console.WriteLine($" [x] Received {msg}");

};

channel.BasicConsume(queue:queuName, autoAck:false, consumer:consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();