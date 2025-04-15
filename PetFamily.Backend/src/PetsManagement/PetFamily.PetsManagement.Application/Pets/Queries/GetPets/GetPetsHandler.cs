using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using System.Text;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPets;

public class GetPetsHandler : IQueryHandler<DataListPage<PetDTO>, GetPetsQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetPetsQueryValidator _validator;
    private readonly ILogger<GetPetsHandler> _logger;

    public GetPetsHandler(
        IDBConnectionFactory dBConnectionFactory,
        GetPetsQueryValidator validator,
        ILogger<GetPetsHandler> logger)
    {
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<DataListPage<PetDTO>, ErrorList>> HandleAsync(
        GetPetsQuery query,
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
        var sqlFiltering = СreateFilteringSQL(parameters, query);

        var sqlForCounting = new StringBuilder("""
            SELECT Count(Pets.id)
            FROM Pets
            """);
        sqlForCounting.Append(sqlFiltering);
        var totalCount = await connection.ExecuteScalarAsync<int>(sqlForCounting.ToString(), parameters);

        // after count excuted, reuse the SAME string builder with different SELECT statement
        var sqlForSelecting = new StringBuilder("""
            SELECT
                id,
                name,
                description,
                color,
                weight,
                height,
                breed_id,
                species_id,
                health_information,
                city,
                street,
                house_number,
                house_subnumber,
                appartment_number,
                owner_phone,
                is_castrated,
                birth_date,
                is_vacinated,
                status,
                serial_number,
                volunteer_id,
                is_deleted
            FROM Pets
            """);
        sqlForSelecting.Append(sqlFiltering);
        parameters.Add("@offset", (query.CurrentPage - 1) * query.PageSize);
        parameters.Add("@limit", query.PageSize);

        if (query.Sort is not null && query.Sort.Any())
            DapperSQLHelper.ApplySorting(sqlForSelecting, query.Sort);

        DapperSQLHelper.ApplyPagination(sqlForSelecting, parameters, query.CurrentPage, query.PageSize);

        _logger.LogInformation("Dapper SQL: {sql}", sqlForSelecting.ToString());

        var entities = await connection.QueryAsync<PetDTO>(sqlForSelecting.ToString(), parameters);

        var result = new DataListPage<PetDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<PetDTO>, ErrorList>(result);
    }

    private StringBuilder СreateFilteringSQL(
        DynamicParameters parameters,
        GetPetsQuery query)
    {
        var sql = new StringBuilder(" WHERE is_deleted = false");

        if (string.IsNullOrEmpty(query.Name) == false)
        {
            parameters.Add("@name", query.Name);
            sql.Append(" AND name like '%@name%'");
        }
        if (string.IsNullOrEmpty(query.Color) == false)
        {
            parameters.Add("@color", query.Color);
            sql.Append(" AND color = '%@color%'");
        }
        if (query.MinWeight is not null)
        {
            parameters.Add("@minweight", query.MinWeight);
            sql.Append(" AND weight >= @minweight");
        }
        if (query.MaxWeight is not null)
        {
            parameters.Add("@maxweight", query.MaxWeight);
            sql.Append(" AND weight <= @maxweight");
        }
        if (query.MinHeight is not null)
        {
            parameters.Add("@minheight", query.MinHeight);
            sql.Append(" AND height >= @minHeight");
        }
        if (query.MaxHeight is not null)
        {
            parameters.Add("@maxHeight", query.MaxHeight);
            sql.Append(" AND height <= @maxHeight");
        }
        if (query.Breed_Id is not null)
        {
            parameters.Add("@breedid", query.Breed_Id);
            sql.Append(" AND breed_id = @breedid");
        }
        if (query.Species_Id is not null)
        {
            parameters.Add("@speciesid", query.Species_Id);
            sql.Append(" AND species_id = @speciesid");
        }
        if (string.IsNullOrEmpty(query.Health_Information) == false)
        {
            parameters.Add("@health_information", query.Health_Information);
            sql.Append(" AND health_information like '%@health_information%'");
        }
        if (string.IsNullOrEmpty(query.City) == false)
        {
            parameters.Add("@city", query.City);
            sql.Append(" AND city like '%@city%'");
        }
        if (string.IsNullOrEmpty(query.Street) == false)
        {
            parameters.Add("@street", query.Street);
            sql.Append(" AND street like '%@street%'");
        }
        if (string.IsNullOrEmpty(query.House_Number) == false)
        {
            parameters.Add("@house_number", query.House_Number);
            sql.Append(" AND house_number like '%@house_number%'");
        }
        if (string.IsNullOrEmpty(query.House_SubNumber) == false)
        {
            parameters.Add("@house_subnumber", query.House_SubNumber);
            sql.Append(" AND house_subnumber like '%@house_subnumber%'");
        }
        if (string.IsNullOrEmpty(query.Appartment_Number) == false)
        {
            parameters.Add("@appartment_number", query.Appartment_Number);
            sql.Append(" AND appartment_number like '%@appartment_number%'");
        }
        if (string.IsNullOrEmpty(query.Owner_Phone) == false)
        {
            parameters.Add("@owner_phone", query.Owner_Phone);
            sql.Append(" AND owner_phone like '%@owner_phone%'");
        }
        if (query.Is_Castrated is not null)
        {
            parameters.Add("@is_castrated", query.Is_Castrated);
            sql.Append(" AND is_castrated = @is_castrated");
        }
        if (query.MinAge is not null)
        {
            var maxBirthDate = DateTime.UtcNow.AddYears(-(int)query.MinAge);

            parameters.Add("@maxbirthdate", maxBirthDate);
            sql.Append(" AND birth_date <= @maxbirthdate");
        }
        if (query.MaxAge is not null)
        {
            var minBirthDate = DateTime.UtcNow.AddYears(-(int)query.MaxAge);

            parameters.Add("@minbirthdate", minBirthDate);
            sql.Append(" AND birth_date >= @minbirthdate");
        }
        if (query.Is_Vacinated is not null)
        {
            parameters.Add("@is_vacinated", query.Is_Vacinated);
            sql.Append(" AND is_vacinated = @is_vacinated");
        }
        if (query.Status is not null)
        {
            parameters.Add("@status", query.Status);
            sql.Append(" AND status = @status");
        }
        if (query.Volunteer_Id is not null)
        {
            parameters.Add("@volunteer_id", query.Volunteer_Id);
            sql.Append(" AND volunteer_id = @volunteer_id");
        }

        return sql;
    }
}
