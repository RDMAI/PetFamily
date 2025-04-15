using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace PetFamily.Shared.Kernel.ValueObjects;

public record SocialNetwork
{
    public string Name { get; }
    public string Link { get; }

    public static Result<SocialNetwork, Error> Create(string name, string link)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Social Network Name");
        if (string.IsNullOrWhiteSpace(link) || link.Length > Constants.MAX_MID_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Social Network Link");

        return new SocialNetwork(name, link);
    }

    [JsonConstructor]
    private SocialNetwork(string name, string link)
    {
        Name = name;
        Link = link;
    }
}
