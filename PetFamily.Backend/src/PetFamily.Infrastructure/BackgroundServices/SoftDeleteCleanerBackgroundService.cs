using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Infrastructure.DataBaseAccess.Write;
using PetFamily.Infrastructure.Options;

namespace PetFamily.Infrastructure.BackgroundServices;

public class SoftDeleteCleanerBackgroundService : BackgroundService
{
    private readonly IDbContextFactory<WriteDBContext> _dbFactory;
    private readonly ILogger<SoftDeleteCleanerBackgroundService> _logger;

    private readonly TimeSpan _checkPeriod;
    private readonly TimeSpan _timeToRestore;

    public SoftDeleteCleanerBackgroundService(
        IDbContextFactory<WriteDBContext> dbFactory,
        ILogger<SoftDeleteCleanerBackgroundService> logger,
        IOptions<SoftDeleteCleanerOptions> options)
    {
        _dbFactory = dbFactory;
        _logger = logger;
        if (options != null)
        {
            _checkPeriod = TimeSpan.FromHours(options.Value.CheckPeriodHours);
            _timeToRestore = TimeSpan.FromHours(options.Value.TimeToRestoreHours);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SoftDeleteCleanerBackgroundService started");

        using PeriodicTimer timer = new(_checkPeriod);

        // when stop requested, waiting for timer to tick will be cancelled too
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var context = _dbFactory.CreateDbContext();

                var deletedVolunteers = context.Volunteers
                    .Where(v => v.IsDeleted &&
                        DateTime.UtcNow - v.DeletionDate!.Value >= _timeToRestore);

                // this will also cascade delete related pets
                context.Volunteers.RemoveRange(deletedVolunteers);

                await context.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("SoftDeleteCleanerBackgroundService executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation("SoftDeleteCleanerBackgroundService finished");
    }
}
