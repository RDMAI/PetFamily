using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PetFamily.Infrastructure.BackgroundServices;

public class SoftDeleteCleanerOptions
{
    public TimeSpan CheckPeriod { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan TimeToRestore { get; set; } = TimeSpan.FromDays(30);
}

public class SoftDeleteCleanerBackgroundService : BackgroundService
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<SoftDeleteCleanerBackgroundService> _logger;

    private readonly TimeSpan _checkPeriod;
    private readonly TimeSpan _timeToRestore;

    public SoftDeleteCleanerBackgroundService(
        IDbContextFactory<ApplicationDBContext> dbFactory,
        ILogger<SoftDeleteCleanerBackgroundService> logger,
        IOptions<SoftDeleteCleanerOptions> options)
    {
        _context = dbFactory.CreateDbContext();
        _logger = logger;
        if (options != null)
        {
            _checkPeriod = options.Value.CheckPeriod;
            _timeToRestore = options.Value.TimeToRestore;
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
                var deletedVolunteers = _context.Volunteers
                    .Where(v => v.IsDeleted &&
                        DateTime.UtcNow - v.DeletionDate!.Value >= _timeToRestore);

                // this will also cascade delete related pets
                _context.Volunteers.RemoveRange(deletedVolunteers);

                await _context.SaveChangesAsync(stoppingToken);

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
