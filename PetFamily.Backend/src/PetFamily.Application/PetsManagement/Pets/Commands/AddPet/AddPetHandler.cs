using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Primitives;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using System.Text;

namespace PetFamily.Application.PetsManagement.Pets.Commands.AddPet;

public class AddPetHandler
    : ICommandHandler<PetId, AddPetCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly AddPetCommandValidator _validator;
    private readonly ILogger<AddPetHandler> _logger;

    public AddPetHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IDBConnectionFactory dBConnectionFactory,
        AddPetCommandValidator validator,
        ILogger<AddPetHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
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
        var breedResult = await GetBreedByIdAsync(breedId.Value, cancellationToken);
        if (breedResult.IsFailure)
            return breedResult.Error;

        var speciesId = SpeciesId.Create(breedResult.Value.SpeciesId);
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

    public async Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@id", BreedId);

        var sql = new StringBuilder(
            """
            SELECT id, name, species_id
            FROM Breeds
            WHERE id = @id
            LIMIT 1
            """
        );

        var entity = await connection.QueryFirstAsync<BreedDTO>(sql.ToString(), parameters);

        if (entity == null)
            return ErrorHelper.General.NotFound(BreedId).ToErrorList();

        return Result.Success<BreedDTO, ErrorList>(entity);
    }
}
