﻿using CSharpFunctionalExtensions;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.Interfaces;
public interface IVolunteerRepository
{
    Task<Result<Volunteer, ErrorList>> GetByIdAsync(
        VolunteerId Id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns error if volunteer with the same 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UnitResult<ErrorList>> IsEmailNotExistAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task<UnitResult<ErrorList>> IsPhoneNotExistAsync(
        Phone phone,
        CancellationToken cancellationToken = default);

    Task<Result<VolunteerId, ErrorList>> CreateAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default);

    Task<Result<VolunteerId, ErrorList>> UpdateAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default);

    Task<Result<VolunteerId, ErrorList>> HardDeleteAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default);

    Task<Result<VolunteerId, ErrorList>> SoftDeleteAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default);
}
