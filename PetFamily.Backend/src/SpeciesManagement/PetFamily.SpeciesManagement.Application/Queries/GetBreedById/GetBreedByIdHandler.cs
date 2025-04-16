using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;
using System.Text;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreedById;

public class GetBreedByIdHandler
    : IQueryHandler<BreedDTO, GetBreedByIdQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetBreedByIdQueryValidator _validator;
    private readonly ILogger<GetBreedByIdHandler> _logger;

    public GetBreedByIdHandler(
        [FromKeyedServices(DependencyKey.Species)] IDBConnectionFactory dBConnectionFactory,
        GetBreedByIdQueryValidator validator,
        ILogger<GetBreedByIdHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<BreedDTO, ErrorList>> HandleAsync(
        GetBreedByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        // query validation
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!validatorResult.IsValid)
        {
            var errors = from e in validatorResult.Errors
                         select Error.Deserialize(e.ErrorMessage);
            return new ErrorList(errors);
        }

        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@id", query.BreedId);

        var sql = new StringBuilder(
            """
            SELECT *
            FROM Breeds
            WHERE id = @id
            """);

        var result = await connection.QueryFirstOrDefaultAsync<BreedDTO>(sql.ToString(), parameters);
        if (result is null)
            return ErrorHelper.General.NotFound(query.BreedId).ToErrorList();

        return Result.Success<BreedDTO, ErrorList>(result);
    }
}
