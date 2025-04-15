using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.DeleteVolunteer;
public record DeleteVolunteerCommand(
    Guid VolunteerId) : ICommand;
