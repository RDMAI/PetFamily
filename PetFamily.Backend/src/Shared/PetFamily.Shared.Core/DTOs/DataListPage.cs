namespace PetFamily.Shared.Core.DTOs;

public class DataListPage<T>
{
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public IEnumerable<T> Data { get; init; }
}
