using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Read.Helpers;
using System.Text;

namespace PetFamily.Infrastructure.DataBaseAccess.Read.DBReaders;

public class SpeciesAggregateDBReader : ISpeciesAggregateDBReader
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly ILogger<SpeciesAggregateDBReader> _logger;

    public SpeciesAggregateDBReader(
        IDBConnectionFactory dBConnectionFactory,
        ILogger<SpeciesAggregateDBReader> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<DataListPage<SpeciesDTO>, ErrorList>> GetAsync(
        GetSpeciesQuery query,
        CancellationToken cancellationToken = default)
    {
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

    public async Task<Result<SpeciesDTO, ErrorList>> GetByIdAsync(
        Guid SpeciesId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@id", SpeciesId);

        var sql = new StringBuilder(
            """
            SELECT id, name
            FROM Species
            WHERE id = @id
            LIMIT 1
            """
        );

        var entity = await connection.QueryFirstAsync<SpeciesDTO>(sql.ToString(), parameters);

        if (entity == null)
            return ErrorHelper.General.NotFound(SpeciesId).ToErrorList();

        return Result.Success<SpeciesDTO, ErrorList>(entity);
    }

    public async Task<Result<DataListPage<BreedDTO>, ErrorList>> GetBreedsAsync(
        GetBreedsQuery query,
        CancellationToken cancellationToken = default)
    {
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

    public async Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@id", BreedId);

        var sql = new StringBuilder(
            """
            SELECT id, name, species_id
            FROM Breeds
            WHERE id = @id
            LIMIT 1
            """
        );

        var entity = await connection.QueryFirstAsync<BreedDTO>(sql.ToString(), parameters);

        if (entity == null)
            return ErrorHelper.General.NotFound(BreedId).ToErrorList();

        return Result.Success<BreedDTO, ErrorList>(entity);
    }

    public async Task<UnitResult<ErrorList>> ArePetsWithBreedIdNotExistAsync(
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@breedId", BreedId);

        var sql = new StringBuilder(
            """
            SELECT id
            FROM Pets
            WHERE breed_id = @breedId
            LIMIT 1
            """
        );

        var result = await connection.QueryAsync<Guid>(sql.ToString(), parameters);
        if (result is not null && result.Any())
            return Error.Conflict(
                "relation.exist",
                $"Cannot delete breed {BreedId}. It has related pet: {result.First()}")
                .ToErrorList();

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> ArePetsWithSpeciesIdNotExistAsync(
        Guid SpeciesId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@speciesId", SpeciesId);

        var sql = new StringBuilder(
            """
            SELECT id
            FROM Pets
            WHERE species_id = @speciesId
            LIMIT 1
            """
        );

        var result = await connection.QueryAsync<Guid>(sql.ToString(), parameters);
        if (result is not null && result.Any())
            return Error.Conflict(
                "relation.exist",
                $"Cannot delete species {SpeciesId}. It has related pet: {result.First()}")
                .ToErrorList();

        return UnitResult.Success<ErrorList>();
    }
}
