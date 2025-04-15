using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Shared.Core.Abstractions;
using System.Data;

namespace PetFamily.SpeciesManagement.Infrastructure.Database.Write;

public class SpeciesUnitOfWork : IUnitOfWork
{
    private readonly SpeciesWriteDBContext _dbContext;

    public SpeciesUnitOfWork(SpeciesWriteDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
