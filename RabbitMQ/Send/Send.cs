using System.Text;
using RabbitMQ.Client;
internal class Program
{
    static void Main(string[] args)
    {
        //Implement the Connection, u can chanage the localhost by the server ip
        var factory = new ConnectionFactory{ HostName = "localhost"};
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("hello", durable:false, exclusive:false, autoDelete:false, arguments:null);
        string msg = "";
        int x = 10;
        while (x >=0)
        {
            msg = "Hello World!";
            msg += $" {x}";
            var body = Encoding.UTF8.GetBytes(msg);
            Task.Delay(1000).Wait();
            channel.BasicPublish(exchange:string.Empty, routingKey:"hello",basicProperties: null, body:body);
            x--;
        }

        Console.WriteLine($" [x] Sent {msg}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}