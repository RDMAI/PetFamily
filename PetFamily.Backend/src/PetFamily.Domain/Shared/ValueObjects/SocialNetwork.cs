using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
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

    public static Result<SocialNetwork, Error> Create(string name, string link)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ErrorHelper.General.ValueIsNullOrEmpty("Name");
        if (string.IsNullOrWhiteSpace(link))
            return ErrorHelper.General.ValueIsNullOrEmpty("Link");

        return new SocialNetwork(name, link);
    }

    private SocialNetwork(string name, string link)
    {
        Name = name;
        Link = link;
    }
}
