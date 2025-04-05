using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Shared.Validation;
public static class CustomValidator
{
    /// <summary>
    /// Validates Property from DTO to be a valid ValueObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TValueObject"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <param name="factoryMethod"></param>
    /// <returns></returns>
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, Error> result = factoryMethod(value);

            if (result.IsSuccess) return;

            context.AddFailure(result.Error.Serialize());
        });
    }

    public static IRuleBuilderOptionsConditions<T, TElement> MustBeNotNull<T, TElement>(
        this IRuleBuilder<T, TElement> ruleBuilder)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            if (value is not null)
                return;

            var error = ErrorHelper.General.ValueIsNull();

            context.AddFailure(error.Serialize());
        });
    }

    public static IRuleBuilderOptionsConditions<T, IEnumerable<SortByDTO>?> MustBeValidSorting<T>(
        this IRuleBuilder<T, IEnumerable<SortByDTO>?> ruleBuilder,
        IEnumerable<string> propertyNames)
    {
        return ruleBuilder.Custom((sortList, context) =>
        {
            if (sortList is null) return;
            foreach (var sort in sortList)
                if (propertyNames.Contains(sort.Property, StringComparer.OrdinalIgnoreCase) == false)
                {
                    context.AddFailure(ErrorHelper.General.ValueIsInvalid($"Property: {sort.Property}").Serialize());
                }
        });
    }

    public static IRuleBuilderOptionsConditions<T, IEnumerable<SortByDTO>?> MustBeValidSorting<T>(
        this IRuleBuilder<T, IEnumerable<SortByDTO>?> ruleBuilder,
        Type type)
    {
        var props = type.GetProperties().Select(p => p.Name);

        return MustBeValidSorting(ruleBuilder, props);
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
    {
        return rule.WithMessage(error.Serialize());
    }
}
