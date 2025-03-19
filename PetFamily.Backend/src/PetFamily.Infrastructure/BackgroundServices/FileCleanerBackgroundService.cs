using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.Shared.Messaging;

namespace PetFamily.Infrastructure.BackgroundServices;

public class FileCleanerBackgroundService : BackgroundService
{
    public const int TIME_BEFORE_RETRY = 100000;
    public const int RETRY_ATTEMPTS_COUNT = 10;

    private readonly ILogger<FileCleanerBackgroundService> _logger;
    private readonly IMessageQueue<IEnumerable<FileInfoDTO>> _fileMessageQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FileCleanerBackgroundService(
        ILogger<FileCleanerBackgroundService> logger,
        IMessageQueue<IEnumerable<FileInfoDTO>> fileMessageQueue,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _fileMessageQueue = fileMessageQueue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MinioFileCleanerBackgroundService started");

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var fileProvider = scope.ServiceProvider.GetRequiredService<IFileProvider>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // waits until queue has any messages
                var fileInfos = await _fileMessageQueue.ReadAsync(stoppingToken);

                Task.Run(() => AttemptFilesDelete(fileInfos, fileProvider, stoppingToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation("MinioFileCleanerBackgroundService finished");
    }

    private async Task AttemptFilesDelete(
        IEnumerable<FileInfoDTO> fileInfos,
        IFileProvider fileProvider,
        CancellationToken cancellationToken = default)
    {
        var counter = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await fileProvider.DeleteFilesAsync(fileInfos, cancellationToken);

            if (result.IsSuccess) return;

            if (counter >= RETRY_ATTEMPTS_COUNT)
            {
                _logger.LogError("Cannot delete files after {count} attempts", RETRY_ATTEMPTS_COUNT);
                break;
            }

            var retrySeconds = TIME_BEFORE_RETRY / 1000;
            _logger.LogWarning("Cannot delete files (attempt = {counter}). Retrying in {time} seconds.", counter, retrySeconds);

            counter++;

            await Task.Delay(TIME_BEFORE_RETRY, cancellationToken);
        }

        
    }
}
