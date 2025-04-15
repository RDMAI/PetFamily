using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;
using System.Text;

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreeds;

public class GetBreedsHandler
    : IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetBreedsQueryValidator _validator;
    private readonly ILogger<GetBreedsHandler> _logger;

    public GetBreedsHandler(
        IDBConnectionFactory dBConnectionFactory,
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

        var parameters = new DynamicParameters();
        parameters.Add("@speciesId", query.SpeciesId);

        var totalCount = await connection.ExecuteScalarAsync<int>("""
            SELECT Count(*)
            FROM Breeds
            WHERE species_id = @speciesId
            """, parameters);

        var sql = new StringBuilder(
            """
            SELECT id, name, species_id
            FROM Breeds
            WHERE species_id = @speciesId
            """);

        if (query.Sort != null && query.Sort.Any())
            DapperSQLHelper.ApplySorting(sql, query.Sort);
        DapperSQLHelper.ApplyPagination(sql, parameters, query.CurrentPage, query.PageSize);

        _logger.LogInformation("Dapper SQL: {sql}", sql.ToString());

        var entities = await connection.QueryAsync<BreedDTO>(sql.ToString(), parameters);

        var result = new DataListPage<BreedDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<BreedDTO>, ErrorList>(result);
    }
}
