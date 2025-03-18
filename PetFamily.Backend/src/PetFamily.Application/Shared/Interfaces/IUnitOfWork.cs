using System.Data;

namespace PetFamily.Application.Shared.Interfaces;
public interface IUnitOfWork
{
    public Task<IDbTransaction> BeginTransaction(CancellationToken cancellationToken = default);
}
