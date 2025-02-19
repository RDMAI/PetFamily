using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;

namespace PetFamily.Domain.Shared.ValueObjects;
public record SocialNetworkList
{
    public IReadOnlyList<SocialNetwork> List { get; } = [];

    public static Result<SocialNetworkList, Error> Create(IEnumerable<SocialNetwork> value)
    {
        if (value is null)
            return ErrorHelper.General.ValueIsNull("Social Networks");

        return new SocialNetworkList(value);
    }

    // EF Core
    private SocialNetworkList() { }
    private SocialNetworkList(IEnumerable<SocialNetwork> value)
    {
        List = (IReadOnlyList<SocialNetwork>)value;
    }
}
