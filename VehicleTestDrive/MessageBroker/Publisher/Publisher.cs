
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MessageBroker.Publisher
{
    public class Publisher<T> : IPublisher<T>
    {

        private readonly RabbitMQSettings _rabbitMqSetting;

        public Publisher(IOptions<RabbitMQSettings> settings)
        {
            _rabbitMqSetting = settings.Value;
        }

        public async Task PublishMessageAsync(T message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSetting.HostName,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
            };

            using var connection = factory.CreateConnectionAsync().Result;
            using var channel = connection.CreateChannelAsync().Result;
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);

        }
    }

}
