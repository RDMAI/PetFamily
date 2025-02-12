using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Requisites
{
    // Реквизиты для помощи (у каждого реквизита будет название и описание, как сделать перевод), поэтому нужно сделать отдельный класс для реквизита.
    // Это должен быть Value Object
    public string Name { get; }
    public string Description { get; }
    public string Value { get; }

    public static Result<Requisites> Create(string name, string description, string value)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Requisites>("City name cannot be empty.");
        if (string.IsNullOrWhiteSpace(description)) return Result.Failure<Requisites>("Street name cannot be empty.");
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure<Requisites>("Street name cannot be empty.");

        return Result.Success(new Requisites(name, description, value));
    }

    private Requisites(string name, string description, string value)
    {
        Name = name;
        Description = description;
        Value = value;
    }
}
