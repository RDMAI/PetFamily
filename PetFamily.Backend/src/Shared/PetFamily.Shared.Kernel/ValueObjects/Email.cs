﻿using CSharpFunctionalExtensions;

namespace PetFamily.Shared.Kernel.ValueObjects;

public record Email
{
    public string Value { get; }

    public static Result<Email, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_LOW_TEXT_LENGTH)  // add proper email validation
            return ErrorHelper.General.ValueIsInvalid("Email");

        return new Email(value);
    }

    private Email(string value)
    {
        Value = value;
    }

    // EF Core
    private Email() { }
}
