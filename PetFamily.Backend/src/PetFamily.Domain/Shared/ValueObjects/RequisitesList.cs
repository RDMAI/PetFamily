using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;

namespace PetFamily.Domain.Shared.ValueObjects;
public record RequisitesList
{
    public IReadOnlyList<Requisites> List { get; } = [];

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
        List = (IReadOnlyList<Requisites>)value;
    }
}
