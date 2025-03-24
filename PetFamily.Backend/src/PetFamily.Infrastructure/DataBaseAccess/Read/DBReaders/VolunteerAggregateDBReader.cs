using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure.DataBaseAccess.Read.Helpers;
using System.Text;

namespace PetFamily.Infrastructure.DataBaseAccess.Read.DBReaders;
public class VolunteerAggregateDBReader : IVolunteerAggregateDBReader
{
    private readonly IDBConnectionFactory _dBConnectionFactory;

    public VolunteerAggregateDBReader(IDBConnectionFactory dBConnectionFactory)
    {
        _dBConnectionFactory = dBConnectionFactory;
    }

    public async Task<Result<DataListPage<VolunteerDTO>, ErrorList>> GetAsync(
        GetVolunteersQuery query,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var totalCount = await connection.ExecuteScalarAsync<int>("""
            SELECT Count(*)
            FROM Volunteers
            WHERE is_deleted = false
            """);

        var parameters = new DynamicParameters();
        parameters.Add("@offset", (query.CurrentPage - 1) * query.PageSize);
        parameters.Add("@limit", query.PageSize);

        var sql = new StringBuilder(
            """
            SELECT id, first_name, last_name, father_name, email, description, experience_years, phone, requisites, social_networks, is_deleted
            FROM Volunteers
            WHERE is_deleted = false
            """);

        if (query.Sort.Any())
            DapperSQLHelper.ApplySorting(sql, query.Sort);

        DapperSQLHelper.ApplyPagination(sql, parameters, query.CurrentPage, query.PageSize);

        var entities = await connection.QueryAsync<VolunteerDTO>(sql.ToString(), parameters);

        var result = new DataListPage<VolunteerDTO>
        {
            TotalCount = totalCount,
            PageNumber = query.CurrentPage,
            PageSize = query.PageSize,
            Data = entities
        };

        return Result.Success<DataListPage<VolunteerDTO>, ErrorList>(result);
    }

    public async Task<Result<VolunteerDTO, ErrorList>> GetByIdAsync(
        GetVolunteerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@id", query.Id);

        var sql = new StringBuilder(
            """
            SELECT id, first_name, last_name, father_name, email, description, experience_years, phone, requisites, social_networks, is_deleted
            FROM Volunteers
            WHERE id = @id and is_deleted = false
            LIMIT 1
            """);

        var entity = await connection.QueryFirstAsync<VolunteerDTO>(sql.ToString(), parameters);

        if (entity == null)
            return ErrorHelper.General.NotFound(query.Id).ToErrorList();

        return Result.Success<VolunteerDTO, ErrorList>(entity);
    }
}
