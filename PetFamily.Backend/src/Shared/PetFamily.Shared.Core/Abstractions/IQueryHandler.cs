using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Shared.Core.Abstractions;

public interface IQueryHandler<TResponse, TQuery> where TQuery : IQuery
{
    public Task<Result<TResponse, ErrorList>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}
