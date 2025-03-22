using CSharpFunctionalExtensions;
using Dapper;
using FluentValidation;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;

public class GetVolunteersHandler
    : IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetVolunteersQueryValidator _validator;

    public GetVolunteersHandler(
        IDBConnectionFactory dBConnectionFactory,
        GetVolunteersQueryValidator validator)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
    }

    public async Task<Result<DataListPage<VolunteerDTO>, ErrorList>> HandleAsync(
        GetVolunteersQuery query,
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

        var totalCount = await connection.ExecuteScalarAsync<int>("""
            SELECT Count(*)
            FROM Volunteers
            """);

        var parameters = new DynamicParameters();
        parameters.Add("@offset", (query.CurrentPage - 1) * query.PageSize);
        parameters.Add("@limit", query.PageSize);

        var sql = """
            SELECT id, first_name, last_name, father_name, email, description, experience_years, phone, requisites, social_networks, is_deleted
            FROM Volunteers
            WHERE is_deleted = false
            ORDER BY last_name
            LIMIT @limit OFFSET @offset
            """;

        var volunteers = await connection.QueryAsync<VolunteerDTO>(sql, parameters);

        var result = new DataListPage<VolunteerDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = volunteers
        };

        return Result.Success<DataListPage<VolunteerDTO>, ErrorList>(result);
    }
}
