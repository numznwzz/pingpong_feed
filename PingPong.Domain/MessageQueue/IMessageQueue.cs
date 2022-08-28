namespace PingPong.Domain.MessageQueue
{
    public interface IMessageQueue<T>
    {
        Task PublishAsync<T>(Object obj) where T : class;
    }
}