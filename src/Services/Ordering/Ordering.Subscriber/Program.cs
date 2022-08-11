using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var connectionFactory = new ConnectionFactory()
{
    HostName = "localhost"
};

var connection = connectionFactory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare("orders",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (_, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(message);
};

