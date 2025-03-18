using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Application.Shared.Interfaces;
using System.Data;

namespace PetFamily.Infrastructure.EFHelpers;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDBContext _dbContext;

    public EFUnitOfWork(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
