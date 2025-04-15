using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Application.Abstractions;
using System.Text;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;

public class GetVolunteerByIdHandler
    : IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetVolunteerByIdQueryValidator _validator;
    private readonly ILogger<GetVolunteerByIdHandler> _logger;

    public GetVolunteerByIdHandler(
        IDBConnectionFactory dBConnectionFactory,
        GetVolunteerByIdQueryValidator validator,
        ILogger<GetVolunteerByIdHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<VolunteerDTO, ErrorList>> HandleAsync(
        GetVolunteerByIdQuery query,
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
        parameters.Add("@id", query.Id);

        var sql = new StringBuilder(
            """
            SELECT id, first_name, last_name, father_name, email, description, experience_years, phone, requisites, social_networks, is_deleted
            FROM Volunteers
            WHERE id = @id and is_deleted = false
            """);

        var result = await connection.QueryFirstOrDefaultAsync<VolunteerDTO>(sql.ToString(), parameters);
        if (result is null)
            return ErrorHelper.General.NotFound(query.Id).ToErrorList();

        return Result.Success<VolunteerDTO, ErrorList>(result);
    }
}
