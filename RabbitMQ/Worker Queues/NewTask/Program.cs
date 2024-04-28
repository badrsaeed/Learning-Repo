// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName="localhost"};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare(queue: "newTask1", durable: true, exclusive: false, autoDelete: false, arguments: null);

var msg = GetMSG(args);

//Make our msg persistent
var properties = channel.CreateBasicProperties();
properties.Persistent = true;

var body = Encoding.UTF8.GetBytes(msg);

channel.BasicPublish(exchange:string.Empty, body: body, routingKey:"newTask", basicProperties:properties);

Console.WriteLine($" [x] Sent {msg}");

static string GetMSG(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}