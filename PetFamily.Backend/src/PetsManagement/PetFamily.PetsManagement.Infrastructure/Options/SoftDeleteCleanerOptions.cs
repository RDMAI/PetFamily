namespace PetFamily.PetsManagement.Infrastructure.Options;

public class SoftDeleteCleanerOptions
{
    public required float CheckPeriodHours { get; set; }
    public required float TimeToRestoreHours { get; set; }
}
