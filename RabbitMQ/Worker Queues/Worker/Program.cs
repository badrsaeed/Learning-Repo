// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName="localhost"};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare(queue: "newTask1", durable: true, exclusive: false, arguments: null, autoDelete:false);

//to Allow fait dispatch
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine(" [*] Waiting for messages.");


var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var msg = Encoding.UTF8.GetString(body);
    Console.WriteLine(msg);
    Console.WriteLine($" [x] Received {msg}");

    int dots = msg.Split(separator: '.').Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine(" [x] Done");

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

channel.BasicConsume(queue:"newTask", autoAck:false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();