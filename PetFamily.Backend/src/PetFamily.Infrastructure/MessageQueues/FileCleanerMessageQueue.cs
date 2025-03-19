using System.Threading.Channels;

namespace PetFamily.Infrastructure.MessageQueues;

public class FileCleanerMessageQueue
{
    private readonly Channel<string[]> _channel = Channel.CreateUnbounded<string[]>();

    public async Task WriteAsync(string[] paths, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(paths, cancellationToken);
    }

    public async Task<string[]> ReadAsync(CancellationToken cancellationToken = default)
    {
        return [];
    }
}
