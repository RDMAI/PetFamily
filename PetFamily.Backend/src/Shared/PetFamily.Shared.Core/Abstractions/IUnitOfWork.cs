using System.Data;

namespace PetFamily.Shared.Core.Abstractions;
public interface IUnitOfWork
{
    public Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default);
}
