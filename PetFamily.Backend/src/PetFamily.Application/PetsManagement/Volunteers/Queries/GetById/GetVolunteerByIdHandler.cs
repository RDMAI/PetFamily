using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;

public class GetVolunteerByIdHandler
    : IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetVolunteerByIdQueryValidator _validator;

    public GetVolunteerByIdHandler(
        IDBConnectionFactory dBConnectionFactory,
        GetVolunteerByIdQueryValidator validator)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
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

        var sql = """
            SELECT id, first_name, last_name, father_name, email, description, experience_years, phone, requisites, social_networks, is_deleted
            FROM Volunteers
            WHERE id = @id and is_deleted = false
            LIMIT 1
            """;

        var volunteer = await connection.QueryFirstAsync<VolunteerDTO>(sql, parameters);

        if (volunteer == null)
            return ErrorHelper.General.NotFound(query.Id).ToErrorList();

        return Result.Success<VolunteerDTO, ErrorList>(volunteer);
    }
}
