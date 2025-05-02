using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Shared.Core.Abstractions;
using System.Data;

namespace PetFamily.Accounts.Infrastructure.Identity;
public class AccountUnitOfWork : IUnitOfWork
{
    private readonly AccountDBContext _dbContext;

    public AccountUnitOfWork(AccountDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}
