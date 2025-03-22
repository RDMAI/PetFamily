using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using System.Text.Json.Serialization;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Requisites
{
    public string Name { get; }
    public string Description { get; }
    public string Value { get; }

    public static Result<Requisites, Error> Create(string name, string description, string value)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Requisites Name");
        if (string.IsNullOrWhiteSpace(description) || description.Length > Constants.MAX_HIGH_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Requisites Description");
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Requisites Value");

        return new Requisites(name, description, value);
    }

    [JsonConstructor]
    private Requisites(string name, string description, string value)
    {
        Name = name;
        Description = description;
        Value = value;
    }
}
