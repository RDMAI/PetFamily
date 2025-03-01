using CSharpFunctionalExtensions;
using FluentValidation;
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

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
    {
        return rule.WithMessage(error.Serialize());
    }

    /// <summary>
    /// Validates list of DTOs to be valid list of ValueObjects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Tdto"></typeparam>
    /// <typeparam name="TValueObject"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <param name="DTOFactoryMethod"></param>
    /// <returns></returns>
    //public static IRuleBuilderOptionsConditions<T, IEnumerable<Tdto>> MustBeListOfValueObjects<T, Tdto, TValueObject>(
    //    this IRuleBuilder<T, IEnumerable<Tdto>> ruleBuilder,
    //    Func<Tdto, Result<TValueObject, Error>> DTOFactoryMethod)
    //{
    //    return ruleBuilder.Custom((listDTO, context) => {
    //        if (listDTO == null)
    //        {
    //            context.AddFailure(ErrorHelper.General.ValueIsNull().Message);
    //            return;
    //        }

    //        foreach (Tdto dto in listDTO)
    //        {
    //            var resultDTO = DTOFactoryMethod(dto);
    //            if (resultDTO.IsFailure)
    //                context.AddFailure(resultDTO.Error.Message);
    //        }
    //    });
    //}
}
