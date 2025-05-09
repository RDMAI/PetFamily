﻿using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;
using PetFamily.SpeciesManagement.Contracts;
using System.Data;
using System.Text;

namespace PetFamily.SpeciesManagement.Presentation;

public class SpeciesContract : ISpeciesContract
{
    public async Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        IDbConnection connection,
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", BreedId);

        var sql = new StringBuilder(
            """
            SELECT id, name, species_id
            FROM species_management.Breeds
            WHERE id = @id
            """
        );

        var result = await connection.QueryFirstOrDefaultAsync<BreedDTO>(sql.ToString(), parameters);
        if (result is null)
            return ErrorHelper.General.NotFound(BreedId).ToErrorList();

        return Result.Success<BreedDTO, ErrorList>(result);
    }
}
