using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace PetFamily.Shared.Kernel.ValueObjects;

public record File
{
    public string PathToStorage { get; }
    public string Name { get; }

    public static Result<File, Error> Create(string pathToStorage, string name)
    {
        if (string.IsNullOrWhiteSpace(pathToStorage))  // add proper file path validation
            return ErrorHelper.General.ValueIsInvalid("File");

        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("File");

        return new File(pathToStorage, name);
    }

    [JsonConstructor]
    private File(string pathToStorage, string name)
    {
        PathToStorage = pathToStorage;
        Name = name;
    }
}
