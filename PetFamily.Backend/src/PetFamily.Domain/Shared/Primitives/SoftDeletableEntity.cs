using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared.Primitives;
public abstract class SoftDeletableEntity<TId> : Entity<TId> where TId : IComparable<TId>
{
    public static readonly TimeSpan TIME_TO_RESTORE = TimeSpan.FromDays(30);

    // EF Core
    protected SoftDeletableEntity() { }
    protected SoftDeletableEntity(TId id) : base(id) { }

    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletionDate { get; private set; }

    public virtual void Delete()
    {
        IsDeleted = true;
        DeletionDate = DateTime.UtcNow;
    }

    public virtual void Restore()
    {
        IsDeleted = false;
        DeletionDate = null;
    }
}
