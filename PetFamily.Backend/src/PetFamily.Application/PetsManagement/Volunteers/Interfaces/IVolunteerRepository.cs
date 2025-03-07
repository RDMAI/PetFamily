using CSharpFunctionalExtensions;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Volunteers.Interfaces;
public interface IVolunteerRepository
{
    Task<Result<Volunteer, ErrorList>> GetByIdAsync(
        VolunteerId Id,
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

    Task<Result<IEnumerable<Volunteer>, ErrorList>> GetAsync(
        VolunteerFilter filter,
        CancellationToken cancellationToken = default);
}
