using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Volunteers.DTOs;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Validation;
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

            context.AddFailure(result.Error.Message);
        });
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
