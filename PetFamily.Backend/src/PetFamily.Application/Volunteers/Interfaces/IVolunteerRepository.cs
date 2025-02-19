using CSharpFunctionalExtensions;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Volunteers.Interfaces;
public interface IVolunteerRepository
{
    Task<Result<Volunteer, Error>> GetByIdAsync(Guid Id);
    Task<Result<Guid, Error>> CreateAsync(Volunteer entity);
}
