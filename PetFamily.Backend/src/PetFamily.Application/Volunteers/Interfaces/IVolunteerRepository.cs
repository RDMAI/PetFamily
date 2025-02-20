using CSharpFunctionalExtensions;
using PetFamily.Application.Volunteers.Filters;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Volunteers.Interfaces;
public interface IVolunteerRepository
{
    Task<Result<Volunteer, Error>> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    Task<Result<Guid, Error>> CreateAsync(Volunteer entity, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Volunteer>, Error>> GetAsync(VolunteerFilter filter, CancellationToken cancellationToken = default);
}
