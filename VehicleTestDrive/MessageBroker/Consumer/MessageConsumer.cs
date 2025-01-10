using System.Text;
using MessageBroker;
using MessageBroker.Consumer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReservationsApi.Consumer
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly RabbitMQSettings _rabbitMqSetting;

        public MessageConsumer(IOptions<RabbitMQSettings> rabbitMqSetting, IServiceProvider serviceProvider)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
        }

        public async void Receive(string queueName, Func<string, Task> customAction)
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

            await channel.ExchangeDeclareAsync(exchange: string.Empty, type: ExchangeType.Topic);

            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();

            await channel.QueueBindAsync(queue: queueName, exchange: string.Empty, routingKey: queueName);

            var consumer = new AsyncEventingBasicConsumer(channel);
            var message = "";
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                message = Encoding.UTF8.GetString(body);

                //execute what we want to do with this message
                customAction(message);

                var routingKey = ea.RoutingKey;
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
        }

    }

}
