using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory() { HostName="localhost"};
using IConnection connection = factory.CreateConnection();
using IModel channel =  connection.CreateModel();


channel.ExchangeDeclare(exchange: "direct_logs", ExchangeType.Direct);

string queueName = channel.QueueDeclare().QueueName;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

//Make more than binding based on the binding which i get from the user
foreach (var severity in args)
{
    channel.QueueBind(exchange: "direct_logs", queue: queueName, routingKey: severity);
}
Console.WriteLine(" [*] Waiting for messages.");

EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var msg = Encoding.UTF8.GetString(body);

    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{msg}'");
};

channel.BasicConsume(queue:queueName, autoAck:true, consumer:consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();