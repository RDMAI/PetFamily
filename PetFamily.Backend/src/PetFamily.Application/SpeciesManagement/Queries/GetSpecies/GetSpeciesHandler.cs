using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Helpers;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Shared;
using System.Text;

namespace PetFamily.Application.SpeciesManagement.Queries.GetSpecies;

public class GetSpeciesHandler
    : IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetSpeciesQueryValidator _validator;
    private readonly ILogger<GetSpeciesHandler> _logger;

    public GetSpeciesHandler(
        IDBConnectionFactory dBConnectionFactory,
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

        var totalCount = await connection.ExecuteScalarAsync<int>("""
            SELECT Count(*)
            FROM Species
            """);

        var parameters = new DynamicParameters();
        var sql = new StringBuilder(
            """
            SELECT id, name
            FROM Species
            """);

        if (query.Sort.Any())
            DapperSQLHelper.ApplySorting(sql, query.Sort);

        DapperSQLHelper.ApplyPagination(sql, parameters, query.CurrentPage, query.PageSize);

        _logger.LogInformation("Dapper SQL: {sql}", sql.ToString());

        var entities = await connection.QueryAsync<SpeciesDTO>(sql.ToString(), parameters);

        var result = new DataListPage<SpeciesDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<SpeciesDTO>, ErrorList>(result);
    }
}
