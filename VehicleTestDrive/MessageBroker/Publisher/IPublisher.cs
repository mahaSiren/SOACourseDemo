namespace MessageBroker.Publisher
{
    public interface IPublisher<T>
    {
        Task PublishMessageAsync(T message, string queueName);
    }
}