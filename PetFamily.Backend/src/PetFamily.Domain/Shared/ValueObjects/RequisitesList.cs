using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;

namespace PetFamily.Domain.Shared.ValueObjects;
public record RequisitesList
{
    private readonly List<Requisites> _list = [];
    public IReadOnlyList<Requisites> List => _list;

    public static Result<RequisitesList, Error> Create(IEnumerable<Requisites> value)
    {
        if (value is null)
            return ErrorHelper.General.ValueIsNull("Requisites");

        return new RequisitesList(value);
    }

    // EF Core
    private RequisitesList(){}
    private RequisitesList(IEnumerable<Requisites> value)
    {
        _list = value.ToList();
    }
}
