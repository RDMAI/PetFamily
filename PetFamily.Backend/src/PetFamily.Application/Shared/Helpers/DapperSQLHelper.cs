using Dapper;
using PetFamily.Application.Shared.DTOs;
using System.Text;

namespace PetFamily.Application.Shared.Helpers;

public static class DapperSQLHelper
{
    public static void ApplySorting(
        StringBuilder sqlBuilder,
        IEnumerable<SortByDTO> sortList)
    {
        if (!sortList.Any())
            throw new ArgumentException("Invalid order by arguments");

        sqlBuilder.Append(" ORDER BY");

        foreach (var s in sortList)
        {
            sqlBuilder.Append($" {s.Property}");

            var dir = s.IsAscending ? "asc" : "desc";
            sqlBuilder.Append($" {dir},");
        }
        sqlBuilder.Remove(sqlBuilder.Length - 1, 1);  // removes the last ","
    }

    public static void ApplyPagination(
        StringBuilder sqlBuilder,
        DynamicParameters parameters,
        int currentPage,
        int pageSize)
    {
        parameters.Add("@offset", (currentPage - 1) * pageSize);
        parameters.Add("@limit", pageSize);

        sqlBuilder.Append(" LIMIT @limit OFFSET @offset");
    }
}
