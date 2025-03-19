using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.Interfaces;

namespace PetFamily.Infrastructure.BackgroundServices;

public class FileCleanerBackgroundService : BackgroundService
{
    private readonly ILogger<FileCleanerBackgroundService> _logger;
    private readonly IFileProvider _fileProvider;

    public FileCleanerBackgroundService(
        ILogger<FileCleanerBackgroundService> logger,
        IFileProvider fileProvider)
    {
        _logger = logger;
        _fileProvider = fileProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MinioFileCleanerBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //_fileProvider.DeleteFilesAsync(, stoppingToken);



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation("SoftDeleteCleanerBackgroundService finished");
    }
}
