using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPets;

public class GetPetsHandler : IQueryHandler<DataListPage<PetDTO>, GetPetsQuery>
{
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly GetPetsQueryValidator _validator;
    private readonly ILogger<GetPetsHandler> _logger;

    public GetPetsHandler(
        [FromKeyedServices(DependencyKey.Pets)] IDBConnectionFactory dBConnectionFactory,
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

        // Build sql for counting items
        var sqlCount = new CustomSQLBuilder("""
            SELECT Count(Pets.id)
            FROM Pets
            """);
        ApplyFiltering(sqlCount, query);
        var sqlCountString = sqlCount.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlCountString);
        var totalCount = await connection.ExecuteScalarAsync<int>(sqlCountString, sqlCount.Parameters);

        // Build sql for selecting items
        var sqlSelect = new CustomSQLBuilder("""
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
        ApplyFiltering(sqlSelect, query);

        if (query.Sort is not null && query.Sort.Any())
            sqlSelect.ApplySorting(query.Sort);

        sqlSelect.ApplyPagination(query.CurrentPage, query.PageSize);
        var sqlSelectString = sqlSelect.ToString();

        _logger.LogInformation("Dapper SQL: {sql}", sqlSelectString);
        var entities = await connection.QueryAsync<PetDTO>(sqlSelectString, sqlSelect.Parameters);

        var result = new DataListPage<PetDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<PetDTO>, ErrorList>(result);
    }

    private CustomSQLBuilder ApplyFiltering(
        CustomSQLBuilder sqlBuilder,
        GetPetsQuery query)
    {
        sqlBuilder.Append(" WHERE is_deleted = false ");

        if (string.IsNullOrEmpty(query.Name) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("name", query.Name);
        }
        if (string.IsNullOrEmpty(query.Color) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("color", query.Color);
        }
        if (query.MinWeight is not null)
        {
            sqlBuilder.Parameters.Add("@minweight", query.MinWeight);
            sqlBuilder.Append(" AND weight >= @minweight");
        }
        if (query.MaxWeight is not null)
        {
            sqlBuilder.Parameters.Add("@maxweight", query.MaxWeight);
            sqlBuilder.Append(" AND weight <= @maxweight");
        }
        if (query.MinHeight is not null)
        {
            sqlBuilder.Parameters.Add("@minheight", query.MinHeight);
            sqlBuilder.Append(" AND height >= @minHeight");
        }
        if (query.MaxHeight is not null)
        {
            sqlBuilder.Parameters.Add("@maxHeight", query.MaxHeight);
            sqlBuilder.Append(" AND height <= @maxHeight");
        }
        if (query.Breed_Id is not null)
        {
            sqlBuilder.Parameters.Add("@breedid", query.Breed_Id);
            sqlBuilder.Append(" AND breed_id = @breedid");
        }
        if (query.Species_Id is not null)
        {
            sqlBuilder.Parameters.Add("@speciesid", query.Species_Id);
            sqlBuilder.Append(" AND species_id = @speciesid");
        }
        if (string.IsNullOrEmpty(query.Health_Information) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("health_information", query.Health_Information);
        }
        if (string.IsNullOrEmpty(query.City) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("city", query.City);
        }
        if (string.IsNullOrEmpty(query.Street) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("street", query.Street);
        }
        if (string.IsNullOrEmpty(query.House_Number) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("house_number", query.House_Number);
        }
        if (string.IsNullOrEmpty(query.House_SubNumber) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("house_subnumber", query.House_SubNumber);
        }
        if (string.IsNullOrEmpty(query.Appartment_Number) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("appartment_number", query.Appartment_Number);
        }
        if (string.IsNullOrEmpty(query.Owner_Phone) == false)
        {
            sqlBuilder
                .Append("AND")
                .AddTextSearchCondition("owner_phone", query.Owner_Phone);
        }
        if (query.Is_Castrated is not null)
        {
            sqlBuilder.Parameters.Add("@is_castrated", query.Is_Castrated);
            sqlBuilder.Append(" AND is_castrated = @is_castrated");
        }
        if (query.MinAge is not null)
        {
            var maxBirthDate = DateTime.UtcNow.AddYears(-(int)query.MinAge);

            sqlBuilder.Parameters.Add("@maxbirthdate", maxBirthDate);
            sqlBuilder.Append(" AND birth_date <= @maxbirthdate");
        }
        if (query.MaxAge is not null)
        {
            var minBirthDate = DateTime.UtcNow.AddYears(-(int)query.MaxAge);

            sqlBuilder.Parameters.Add("@minbirthdate", minBirthDate);
            sqlBuilder.Append(" AND birth_date >= @minbirthdate");
        }
        if (query.Is_Vacinated is not null)
        {
            sqlBuilder.Parameters.Add("@is_vacinated", query.Is_Vacinated);
            sqlBuilder.Append(" AND is_vacinated = @is_vacinated");
        }
        if (query.Status is not null)
        {
            sqlBuilder.Parameters.Add("@status", query.Status);
            sqlBuilder.Append(" AND status = @status");
        }
        if (query.Volunteer_Id is not null)
        {
            sqlBuilder.Parameters.Add("@volunteer_id", query.Volunteer_Id);
            sqlBuilder.Append(" AND volunteer_id = @volunteer_id");
        }

        return sqlBuilder;
    }
}
