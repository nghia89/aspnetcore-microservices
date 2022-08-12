

using System.Text;
using System.Threading.Channels;
using Contracts.Common.Interfaces;
using Contracts.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messages;

public class RabbitMQProducer : IMessageProducer
{
    private readonly ISerializeService _serializeService;

    public RabbitMQProducer(ISerializeService serializeService)
    {
        _serializeService = serializeService;
    }

    public void SendMessage<T>(T message)
    {
        var connectionFactory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "orders",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
        var jsonData = _serializeService.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonData);

        var properties = channel.CreateBasicProperties();

        properties.Persistent = false;
        channel.BasicPublish("", "orders", properties, body);
    }
}

