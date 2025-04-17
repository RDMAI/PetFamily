using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace PetFamily.Shared.Kernel.ValueObjects;

public record FileVO
{
    public string PathToStorage { get; }
    public string Name { get; }

    public static Result<FileVO, Error> Create(string pathToStorage, string name)
    {
        if (string.IsNullOrWhiteSpace(pathToStorage))  // add proper file path validation
            return ErrorHelper.General.ValueIsInvalid("File");

        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("File");

        return new FileVO(pathToStorage, name);
    }

    [JsonConstructor]
    private FileVO(string pathToStorage, string name)
    {
        PathToStorage = pathToStorage;
        Name = name;
    }

    // EF Core
    private FileVO() { }
}
