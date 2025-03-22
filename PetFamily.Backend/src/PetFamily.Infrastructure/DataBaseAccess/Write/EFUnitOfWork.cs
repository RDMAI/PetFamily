using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Application.Shared.Interfaces;
using System.Data;

namespace PetFamily.Infrastructure.DataBaseAccess.Write;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly WriteDBContext _dbContext;

    public EFUnitOfWork(WriteDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
