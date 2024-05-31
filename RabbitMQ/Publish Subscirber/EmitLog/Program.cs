
using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare("logs", ExchangeType.Fanout);

string msg = GetMessage(args);

byte[] body = Encoding.UTF8.GetBytes(msg);

channel.BasicPublish("logs", string.Empty,basicProperties:null, body: body);

Console.WriteLine($" [x] Sent {msg}");




static string GetMessage(string[] args)
{
    return (args.Length > 0 ? string.Join(", ", args) : "Hello World!");
}