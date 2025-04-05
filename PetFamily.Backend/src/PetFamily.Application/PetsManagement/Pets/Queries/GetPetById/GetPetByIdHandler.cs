using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using System.Text;

namespace PetFamily.Application.PetsManagement.Pets.Queries.GetPetById;

public class GetPetByIdHandler
    : IQueryHandler<PetDTO, GetPetByIdQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetPetByIdQueryValidator _validator;
    private readonly ILogger<GetPetByIdHandler> _logger;

    public GetPetByIdHandler(
        IDBConnectionFactory dBConnectionFactory,
        GetPetByIdQueryValidator validator,
        ILogger<GetPetByIdHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetDTO, ErrorList>> HandleAsync(
        GetPetByIdQuery query,
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
        parameters.Add("@id", query.PetId);

        var sql = new StringBuilder(
            """
            SELECT *
            FROM Pets
            WHERE id = @id and is_deleted = false
            """);

        var result = await connection.QueryFirstOrDefaultAsync<PetDTO>(sql.ToString(), parameters);
        if (result is null)
            return ErrorHelper.General.NotFound(query.PetId).ToErrorList();

        return Result.Success<PetDTO, ErrorList>(result);
    }
}
