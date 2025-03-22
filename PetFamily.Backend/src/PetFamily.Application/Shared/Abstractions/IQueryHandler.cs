using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Shared.Abstractions;

public interface IQueryHandler<TResponse, TQuery> where TQuery : IQuery
{
    public Task<Result<TResponse, ErrorList>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken = default);
}
