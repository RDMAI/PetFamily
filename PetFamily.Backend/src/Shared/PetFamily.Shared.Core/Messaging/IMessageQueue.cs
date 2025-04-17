namespace PetFamily.Shared.Core.Messaging;

public interface IMessageQueue<TMessage>
{
    public Task<TMessage> ReadAsync(CancellationToken cancellationToken = default);
    public Task WriteAsync(TMessage message, CancellationToken cancellationToken = default);
}
