using CSharpFunctionalExtensions;
using PetFamily.PetsManagement.Domain.Entities;
using PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
using PetFamily.Shared.Core;
using PetFamily.Shared.Core.ValueObjects;

namespace PetFamily.PetsManagement.Application.Volunteers.Interfaces;
public interface IVolunteerAggregateRepository
{
    Task<Result<Volunteer, ErrorList>> GetByIdAsync(
        VolunteerId Id,
        CancellationToken cancellationToken = default);
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
