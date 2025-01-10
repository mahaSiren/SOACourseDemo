
namespace MessageBroker.Consumer
{
    public interface IMessageConsumer
    {
        void Receive(string queueName, Func<string, Task> customAction);
    }
}
