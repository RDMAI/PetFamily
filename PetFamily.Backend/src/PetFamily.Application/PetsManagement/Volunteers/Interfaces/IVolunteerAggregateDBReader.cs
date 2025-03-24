using CSharpFunctionalExtensions;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Volunteers.Interfaces;

public interface IVolunteerAggregateDBReader
{
    public Task<Result<DataListPage<VolunteerDTO>, ErrorList>> GetAsync(
        GetVolunteersQuery query,
        CancellationToken cancellationToken = default);

    public Task<Result<VolunteerDTO, ErrorList>> GetByIdAsync(
        GetVolunteerByIdQuery query,
        CancellationToken cancellationToken = default);
}
