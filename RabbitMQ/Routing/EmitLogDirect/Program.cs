using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost"};
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);


string[] logs = ["error", "info", "warning"];
int index = 0;
while (true)
{
    var msg = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : $"{logs[index]} : Hello World!";
    var body = Encoding.UTF8.GetBytes(msg);

    channel.BasicPublish(exchange: "direct_logs", routingKey: logs[index], body: body, basicProperties: null);

    Console.WriteLine($" [x] Sent '{logs[index]}':'{msg}'");
    if(++index == logs.Length)
        index = 0;

    Task.Delay(1000).Wait();
}