﻿using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetColor
{
    public string Value { get; }

    public static Result<PetColor, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ErrorHelper.General.ValueIsNullOrEmpty("Pet color");

        return new PetColor(value);
    }

    private PetColor(string value)
    {
        Value = value;
    }
}
