﻿using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.PetsManagement.Domain.ValueObjects.Pets;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Contracts;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePet;

public class UpdatePetHandler
    : ICommandHandler<PetId, UpdatePetCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly UpdatePetCommandValidator _validator;
    private readonly ILogger<UpdatePetHandler> _logger;
    private readonly ISpeciesContract _speciesProvider;

    public UpdatePetHandler(
        IVolunteerAggregateRepository volunteerRepository,
        [FromKeyedServices(DependencyKey.Pets)] IDBConnectionFactory dBConnectionFactory,
        UpdatePetCommandValidator validator,
        ILogger<UpdatePetHandler> logger,
        ISpeciesContract speciesProvider)
    {
        _volunteerRepository = volunteerRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
        _speciesProvider = speciesProvider;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        UpdatePetCommand command,
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

        // validate pet
        var petId = PetId.Create(command.PetId);
        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return ErrorHelper.General.NotFound(petId.Value).ToErrorList();

        // validate breed if it was changed
        var petBreed = pet.Breed;
        if (petBreed.BreedId.Value != command.Pet.BreedId)
        {
            var breedId = BreedId.Create(command.Pet.BreedId);

            using var connection = _dBConnectionFactory.Create();
            var breedResult = await _speciesProvider.GetBreedByIdAsync(connection, breedId.Value, cancellationToken);
            if (breedResult.IsFailure)
                return breedResult.Error;

            var speciesId = SpeciesId.Create(breedResult.Value.Species_Id);
            petBreed = PetBreed.Create(breedId, speciesId).Value;
        }

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

        // Handle BL
        pet.UpdateFull(petName,
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

        var result = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return petId;
    }
}
