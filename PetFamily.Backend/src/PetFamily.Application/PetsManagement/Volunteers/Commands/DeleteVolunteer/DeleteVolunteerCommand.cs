using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.DeleteVolunteer;
public record DeleteVolunteerCommand(
    Guid VolunteerId) : ICommand;
