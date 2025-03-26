using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using System.Text;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;

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
            LIMIT 1
            """);

        var result = await connection.QueryAsync<VolunteerDTO>(sql.ToString(), parameters);
        if (result is null || result.Any() == false)
            return ErrorHelper.General.NotFound(query.Id).ToErrorList();

        var entity = result.First();

        return Result.Success<VolunteerDTO, ErrorList>(entity);
    }
}
