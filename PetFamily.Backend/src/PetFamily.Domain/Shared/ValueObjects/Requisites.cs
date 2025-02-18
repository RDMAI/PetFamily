using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Requisites
{
    public string Name { get; }
    public string Description { get; }
    public string Value { get; }

    public static Result<Requisites, Error> Create(string name, string description, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ErrorHelper.General.ValueIsNullOrEmpty("Name");
        if (string.IsNullOrWhiteSpace(description))
            return ErrorHelper.General.ValueIsNullOrEmpty("Description");
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty("Value");

        return new Requisites(name, description, value);
    }

    private Requisites(string name, string description, string value)
    {
        Name = name;
        Description = description;
        Value = value;
    }
}
