using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.DeleteVolunteer;
public record DeleteVolunteerCommand(
    Guid VolunteerId) : ICommand;
