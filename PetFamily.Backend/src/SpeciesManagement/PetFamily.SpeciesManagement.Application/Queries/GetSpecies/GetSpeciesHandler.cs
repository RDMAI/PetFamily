using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.SpeciesManagement.Application.Queries.GetSpecies;

public class GetSpeciesHandler
    : IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetSpeciesQueryValidator _validator;
    private readonly ILogger<GetSpeciesHandler> _logger;

    public GetSpeciesHandler(
        [FromKeyedServices(DependencyKey.Species)] IDBConnectionFactory dBConnectionFactory,
        GetSpeciesQueryValidator validator,
        ILogger<GetSpeciesHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<DataListPage<SpeciesDTO>, ErrorList>> HandleAsync(
        GetSpeciesQuery query,
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
            FROM Species
            """);
        ApplyFiltering(sqlCount, query);
        var sqlCountString = sqlCount.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlCountString);
        var totalCount = await connection.ExecuteScalarAsync<int>(sqlCountString, sqlCount.Parameters);

        // Build sql for selecting items
        var sqlSelect = new CustomSQLBuilder("""
            SELECT id, name
            FROM Species
            """);
        ApplyFiltering(sqlSelect, query);

        if (query.Sort is not null && query.Sort.Any())
            sqlSelect.ApplySorting(query.Sort);

        sqlSelect.ApplyPagination(query.CurrentPage, query.PageSize);
        var sqlSelectString = sqlSelect.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlSelect.ToString());
        var entities = await connection.QueryAsync<SpeciesDTO>(sqlSelectString, sqlSelect.Parameters);

        var result = new DataListPage<SpeciesDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<SpeciesDTO>, ErrorList>(result);
    }

    private static CustomSQLBuilder ApplyFiltering(
        CustomSQLBuilder sql,
        GetSpeciesQuery query)
    {
        if (string.IsNullOrEmpty(query.Name) == false)
        {
            sql.Append(" WHERE ")
                .AddTextSearchCondition("name", query.Name);
        }

        return sql;
    }
}
