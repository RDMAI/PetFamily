﻿using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;
using PetFamily.Domain.Shared;
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

    public async Task<Result<DataListPage<BreedDTO>, ErrorList>> GetBreedsAsync(
        GetBreedsQuery query,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var totalCount = await connection.ExecuteScalarAsync<int>("""
            SELECT Count(*)
            FROM Breeds
            WHERE species_id = @speciesId
            """);

        var parameters = new DynamicParameters();
        parameters.Add("@speciesId", query.SpeciesId);

        var sql = new StringBuilder(
            """
            SELECT id, name
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
}
