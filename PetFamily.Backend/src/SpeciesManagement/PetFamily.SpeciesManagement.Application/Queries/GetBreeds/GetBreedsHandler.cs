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

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreeds;

public class GetBreedsHandler
    : IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetBreedsQueryValidator _validator;
    private readonly ILogger<GetBreedsHandler> _logger;

    public GetBreedsHandler(
        [FromKeyedServices(DependencyKey.Species)] IDBConnectionFactory dBConnectionFactory,
        GetBreedsQueryValidator validator,
        ILogger<GetBreedsHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<DataListPage<BreedDTO>, ErrorList>> HandleAsync(
        GetBreedsQuery query,
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
            FROM Breeds
            """);
        ApplyFiltering(sqlCount, query);
        var sqlCountString = sqlCount.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlCountString);
        var totalCount = await connection.ExecuteScalarAsync<int>(sqlCountString, sqlCount.Parameters);

        // Build sql for selecting items
        var sqlSelect = new CustomSQLBuilder("""
            SELECT id, name, species_id
            FROM Breeds
            """);
        ApplyFiltering(sqlSelect, query);

        if (query.Sort is not null && query.Sort.Any())
            sqlSelect.ApplySorting(query.Sort);

        sqlSelect.ApplyPagination(query.CurrentPage, query.PageSize);
        var sqlSelectString = sqlSelect.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlSelect.ToString());
        var entities = await connection.QueryAsync<BreedDTO>(sqlSelectString, sqlSelect.Parameters);

        var result = new DataListPage<BreedDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<BreedDTO>, ErrorList>(result);
    }

    private static CustomSQLBuilder ApplyFiltering(
        CustomSQLBuilder sql,
        GetBreedsQuery query)
    {
        sql.Parameters.Add("@speciesId", query.SpeciesId);
        sql.Append(" WHERE species_id = @speciesId ");

        if (string.IsNullOrEmpty(query.Name) == false)
        {
            sql.Append("AND")
                .AddTextSearchCondition("name", query.Name);
        }

        return sql;
    }
}
