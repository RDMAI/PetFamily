using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.Shared.ValueObjects;

public record SocialNetwork
{
    public string Name { get; }
    public string Link { get; }

    public static Result<SocialNetwork> Create(string name, string link)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<SocialNetwork>("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(link)) return Result.Failure<SocialNetwork>("Link cannot be empty.");

        return Result.Success(new SocialNetwork(name, link));
    }

    private SocialNetwork(string name, string link)
    {
        Name = name;
        Link = link;
    }
}
