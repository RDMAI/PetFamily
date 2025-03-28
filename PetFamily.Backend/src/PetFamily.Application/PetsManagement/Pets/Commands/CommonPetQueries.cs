using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using System.Data;
using System.Text;

namespace PetFamily.Application.PetsManagement.Pets.Commands;

public static class CommonPetQueries
{
    public static async Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        IDbConnection connection,
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", BreedId);

        var sql = new StringBuilder(
            """
            SELECT id, name, species_id
            FROM Breeds
            WHERE id = @id
            """
        );

        var result = await connection.QueryFirstOrDefaultAsync<BreedDTO>(sql.ToString(), parameters);
        if (result is null)
            return ErrorHelper.General.NotFound(BreedId).ToErrorList();

        return Result.Success<BreedDTO, ErrorList>(result);
    }
}
