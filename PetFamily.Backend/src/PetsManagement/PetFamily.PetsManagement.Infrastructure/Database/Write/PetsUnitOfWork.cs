using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Shared.Core.Abstractions;
using System.Data;

namespace PetFamily.PetsManagement.Infrastructure.Database.Write;

public class PetsUnitOfWork : IUnitOfWork
{
    private readonly PetsWriteDBContext _dbContext;

    public PetsUnitOfWork(PetsWriteDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
