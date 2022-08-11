

using System.Text;
using Contracts.Common.Interfaces;
using Contracts.Messages;
using RabbitMQ.Client;

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

        var connection = connectionFactory.CreateConnection();
        using var chanel = connection.CreateModel();
        chanel.QueueDeclare("orders", false);
        var jsonData = _serializeService.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonData);

        var properties = chanel.CreateBasicProperties();

        properties.Persistent = false;
        chanel.BasicPublish("", "orders", properties, body);
    }
}

