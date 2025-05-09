﻿using CSharpFunctionalExtensions;

namespace PetFamily.Shared.Kernel.Abstractions;

public abstract class SoftDeletableEntity<TId> : Entity<TId> where TId : IComparable<TId>
{
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
