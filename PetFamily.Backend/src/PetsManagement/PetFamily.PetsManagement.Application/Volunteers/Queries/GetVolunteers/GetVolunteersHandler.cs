using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;

public class GetVolunteersHandler
    : IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetVolunteersQueryValidator _validator;
    private readonly ILogger<GetVolunteersHandler> _logger;

    public GetVolunteersHandler(
        [FromKeyedServices(DependencyKey.Pets)] IDBConnectionFactory dBConnectionFactory,
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

        // Build sql for counting items
        var sqlCount = new CustomSQLBuilder("""
            SELECT Count(*)
            FROM pets_management.Volunteers
            """);
        ApplyFiltering(sqlCount, query);
        var sqlCountString = sqlCount.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlCountString);
        var totalCount = await connection.ExecuteScalarAsync<int>(sqlCountString, sqlCount.Parameters);

        // Build sql for selecting items
        var sqlSelect = new CustomSQLBuilder("""
            SELECT 
                id,
                first_name,
                last_name,
                father_name,
                email,
                description,
                experience_years,
                phone,
                requisites,
                social_networks,
                is_deleted
            FROM pets_management.Volunteers
            """);
        ApplyFiltering(sqlSelect, query);

        if (query.Sort is not null && query.Sort.Any())
            sqlSelect.ApplySorting(query.Sort);

        sqlSelect.ApplyPagination(query.CurrentPage, query.PageSize);
        var sqlSelectString = sqlSelect.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlSelect.ToString());
        var entities = await connection.QueryAsync<VolunteerDTO>(sqlSelectString, sqlSelect.Parameters);

        var result = new DataListPage<VolunteerDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<VolunteerDTO>, ErrorList>(result);
    }

    private static CustomSQLBuilder ApplyFiltering(
        CustomSQLBuilder sql,
        GetVolunteersQuery query)
    {
        sql.Append(" WHERE is_deleted = false");

        return sql;
    }
}
