using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.PetsManagement.Domain.Entities;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Contracts;

namespace PetFamily.PetsManagement.Application.Pets.Commands.AddPet;

public class AddPetHandler
    : ICommandHandler<PetId, AddPetCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly AddPetCommandValidator _validator;
    private readonly ILogger<AddPetHandler> _logger;
    private readonly ISpeciesContract _speciesProvider;

    public AddPetHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IDBConnectionFactory dBConnectionFactory,
        AddPetCommandValidator validator,
        ILogger<AddPetHandler> logger,
        ISpeciesContract speciesProvider)
    {
        _volunteerRepository = volunteerRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
        _speciesProvider = speciesProvider;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validatorResult.IsValid)
        {
            var errors = from e in validatorResult.Errors
                         select Error.Deserialize(e.ErrorMessage);
            return new ErrorList(errors);
        }

        // validate volunteer
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;
        var volunteer = volunteerResult.Value;

        // validate breed
        var breedId = BreedId.Create(command.Pet.BreedId);
        using var connection = _dBConnectionFactory.Create();

        var breedResult = await _speciesProvider.GetBreedByIdAsync(connection, breedId.Value, cancellationToken);
        if (breedResult.IsFailure)
            return breedResult.Error;

        var speciesId = SpeciesId.Create(breedResult.Value.Species_Id);
        var petBreed = PetBreed.Create(breedId, speciesId).Value;

        var petId = PetId.GenerateNew();
        var petName = PetName.Create(command.Pet.Name).Value;
        var petDescription = Description.Create(command.Pet.Description).Value;
        var petColor = PetColor.Create(command.Pet.Color).Value;
        var petWeight = PetWeight.Create(command.Pet.Weight).Value;
        var petHeight = PetHeight.Create(command.Pet.Height).Value;
        var petHealth = PetHealthInfo.Create(command.Pet.HealthInformation).Value;
        var ownerPhone = Phone.Create(command.Pet.OwnerPhone).Value;
        var petStatus = PetStatus.Create((PetStatuses)command.Pet.Status).Value;

        var address = Address.Create(
            command.Address.City,
            command.Address.Street,
            command.Address.HouseNumber,
            command.Address.HouseSubNumber,
            command.Address.AppartmentNumber).Value;

        List<Requisites> requisitesBufferList = [];
        foreach (RequisitesDTO requisites in command.RequisitesList)
        {
            requisitesBufferList.Add(Requisites.Create(requisites.Name,
                requisites.Description,
                requisites.Value).Value);
        }

        var pet = new Pet(petId,
            petName,
            petDescription,
            petColor,
            petWeight,
            petHeight,
            petBreed,
            petHealth,
            address,
            ownerPhone,
            command.Pet.IsCastrated,
            command.Pet.BirthDate,
            command.Pet.IsVacinated,
            petStatus,
            (ValueObjectList<Requisites>)requisitesBufferList);

        var domainResult = volunteer.AddPet(pet);
        if (domainResult.IsFailure)
            return domainResult.Error.ToErrorList();

        var result = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return petId;
    }
}
