using Dapper;
using PetFamily.Shared.Core.DTOs;
using System.Text;

namespace PetFamily.Shared.Core.Database.Read;

public class CustomSQLBuilder
{
    private readonly StringBuilder _innerBuilder;
    public DynamicParameters Parameters { get; } = new();

    public CustomSQLBuilder()
    {
        _innerBuilder = new();
    }

    public CustomSQLBuilder(string sql)
    {
        _innerBuilder = new(sql);
    }

    public override string ToString()
    {
        return _innerBuilder.ToString();
    }

    public CustomSQLBuilder Append(string sql)
    {
        _innerBuilder.Append(sql);
        return this;
    }

    public CustomSQLBuilder Append(CustomSQLBuilder sqlBuilder)
    {
        _innerBuilder.Append(sqlBuilder._innerBuilder);
        return this;
    }

    /// <summary>
    /// Adds text search condition. This will only add condition itself.
    /// User is responsible for adding "WHERE", "AND", "OR", etc.
    /// Usage example: CustomSQLBuilder
    ///     .Append("WHERE")
    ///     .AddTextSearchCondition("name", "foo")
    ///     .Append("AND")
    ///     .AddTextSearchCondition("last_name", "bar")
    /// </summary>
    /// <param name="propName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public CustomSQLBuilder AddTextSearchCondition(
        string propName,
        string value)
    {
        Parameters.Add(
            name: "@" + propName,
            value: value);
        _innerBuilder.Append($" {propName} like @{propName} || '%' ");

        return this;
    }

    public CustomSQLBuilder ApplySorting(
        IEnumerable<SortByDTO> sortList)
    {
        if (!sortList.Any())
            throw new ArgumentException("Invalid order by arguments");

        _innerBuilder.Append(" ORDER BY");
        foreach (var s in sortList)
        {
            _innerBuilder.Append($" {s.Property.ToLower()}");

            var dir = s.IsAscending ? "asc" : "desc";
            _innerBuilder.Append($" {dir},");
        }
        _innerBuilder.Remove(_innerBuilder.Length - 1, 1);  // removes the last ","

        return this;
    }

    public CustomSQLBuilder ApplyPagination(
        int currentPage,
        int pageSize)
    {
        Parameters.Add("@offset", (currentPage - 1) * pageSize);
        Parameters.Add("@limit", pageSize);

        _innerBuilder.Append(" LIMIT @limit OFFSET @offset");

        return this;
    }
}
