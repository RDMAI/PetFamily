using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using System.Text;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;

public class GetVolunteersHandler
    : IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetVolunteersQueryValidator _validator;
    private readonly ILogger<GetVolunteersHandler> _logger;

    public GetVolunteersHandler(
        IDBConnectionFactory dBConnectionFactory,
        GetVolunteersQueryValidator validator,
        ILogger<GetVolunteersHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
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
            WHERE is_deleted = false
            """);

        var parameters = new DynamicParameters();
        parameters.Add("@offset", (query.CurrentPage - 1) * query.PageSize);
        parameters.Add("@limit", query.PageSize);

        var sql = new StringBuilder(
            """
            SELECT id, first_name, last_name, father_name, email, description, experience_years, phone, requisites, social_networks, is_deleted
            FROM Volunteers
            WHERE is_deleted = false
            """);

        if (query.Sort.Any())
            DapperSQLHelper.ApplySorting(sql, query.Sort);

        DapperSQLHelper.ApplyPagination(sql, parameters, query.CurrentPage, query.PageSize);

        _logger.LogInformation("Dapper SQL: {sql}", sql.ToString());

        var entities = await connection.QueryAsync<VolunteerDTO>(sql.ToString(), parameters);

        var result = new DataListPage<VolunteerDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<VolunteerDTO>, ErrorList>(result);
    }
}
