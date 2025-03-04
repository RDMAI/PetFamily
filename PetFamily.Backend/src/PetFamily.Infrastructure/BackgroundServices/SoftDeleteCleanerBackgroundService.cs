using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetsContext.Entities;

namespace PetFamily.Infrastructure.BackgroundServices;
public class SoftDeleteCleanerBackgroundService : BackgroundService
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<SoftDeleteCleanerBackgroundService> _logger;

    private readonly TimeSpan _period = TimeSpan.FromHours(24);

    public SoftDeleteCleanerBackgroundService(
        IDbContextFactory<ApplicationDBContext> dbFactory,
        ILogger<SoftDeleteCleanerBackgroundService> logger)
    {
        _context = dbFactory.CreateDbContext();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SoftDeleteCleanerBackgroundService started");

        using PeriodicTimer timer = new(_period);

        // when stop requested, waiting for timer to tick will be cancelled too
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var deletedVolunteers = _context.Volunteers
                    .Where(v => v.IsDeleted &&
                        DateTime.UtcNow - v.DeletionDate!.Value >= Volunteer.TIME_TO_RESTORE);

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
