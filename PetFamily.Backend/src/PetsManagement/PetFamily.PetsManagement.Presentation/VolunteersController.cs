using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.PetsManagement.Application.Pets.Commands.AddPet;
using PetFamily.PetsManagement.Application.Pets.Commands.DeletePet;
using PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;
using PetFamily.PetsManagement.Application.Pets.Commands.MovePet;
using PetFamily.PetsManagement.Application.Pets.Commands.SetMainPetPhoto;
using PetFamily.PetsManagement.Application.Pets.Commands.UpdatePet;
using PetFamily.PetsManagement.Application.Pets.Commands.UpdatePetStatus;
using PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.PetsManagement.Application.Pets.Queries.GetPetById;
using PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;
using PetFamily.PetsManagement.Application.Pets.Queries.GetPets;
using PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer;
using PetFamily.PetsManagement.Application.Volunteers.Commands.DeleteVolunteer;
using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;
using PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;
using PetFamily.PetsManagement.Contracts.Requests.Pets;
using PetFamily.PetsManagement.Contracts.Requests.Volunteers;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Framework;
using PetFamily.Shared.Framework.Authorization;
using PetFamily.Shared.Framework.Processors;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Presentation;

[Authorize]
public class VolunteersController : ApplicationController
{
    [Permission(Permissions.Volunteers.READ)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery> volunteersHandler,
        [FromQuery] GetVolunteersRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await volunteersHandler.HandleAsync(
            request.ToQuery(),
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Volunteers.READ)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery> volunteersHandler,
        [FromRoute] Guid id,
        //[FromRoute] GetVolunteerByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await volunteersHandler.HandleAsync(
            new GetVolunteerByIdQuery(id),
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Volunteers.CREATE)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<VolunteerId, CreateVolunteerCommand> volunteerHandler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var createCommand = request.ToCommand();
        var result = await volunteerHandler.HandleAsync(createCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return CreatedBaseURI(volunteerId.Value);
    }

    [Permission(Permissions.Volunteers.UPDATE)]
    [HttpPatch("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfo(
        [FromServices] ICommandHandler<VolunteerId, UpdateMainInfoCommand> volunteerHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateCommand = request.ToCommand(id);
        var result = await volunteerHandler.HandleAsync(updateCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return Ok(volunteerId.Value);
    }

    [Permission(Permissions.Volunteers.UPDATE)]
    [HttpPatch("{id:guid}/social-networks")]
    public async Task<IActionResult> UpdateSocialNetworks(
        [FromServices] ICommandHandler<VolunteerId, UpdateSocialNetworksCommand> volunteerHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateSocialNetworksRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateCommand = request.ToCommand(id);
        var result = await volunteerHandler.HandleAsync(updateCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return Ok(volunteerId.Value);
    }

    [Permission(Permissions.Volunteers.UPDATE)]
    [HttpPatch("{id:guid}/requisites")]
    public async Task<IActionResult> UpdateRequisites(
        [FromServices] ICommandHandler<VolunteerId, UpdateRequisitesCommand> volunteerHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateRequisitesRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateCommand = request.ToCommand(id);
        var result = await volunteerHandler.HandleAsync(updateCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return Ok(volunteerId.Value);
    }

    [Permission(Permissions.Volunteers.DELETE)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] ICommandHandler<VolunteerId, DeleteVolunteerCommand> volunteerHandler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleteCommand = new DeleteVolunteerCommand(id);
        var result = await volunteerHandler.HandleAsync(deleteCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return Ok(volunteerId.Value);
    }

    [Permission(Permissions.Pets.READ)]
    [HttpGet("{volunteerid:guid}/pets")]
    public async Task<IActionResult> GetPets(
        [FromServices] IQueryHandler<DataListPage<PetDTO>, GetPetsQuery> petHandler,
        [FromRoute] Guid volunteerid,
        [FromQuery] GetPetsRequest request,
        CancellationToken cancellationToken = default)
    {
        var getPetsQuery = request.ToQuery(volunteerid);
        var result = await petHandler.HandleAsync(getPetsQuery, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Pets.READ)]
    [HttpGet("{volunteerid:guid}/pets/{petid:guid}")]
    public async Task<IActionResult> GetPetById(
        [FromServices] IQueryHandler<PetDTO, GetPetByIdQuery> petHandler,
        [FromRoute] Guid volunteerid,
        [FromRoute] Guid petid,
        CancellationToken cancellationToken = default)
    {
        var getPetByIdQuery = new GetPetByIdQuery(volunteerid, petid);
        var result = await petHandler.HandleAsync(getPetByIdQuery, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Pets.CREATE)]
    [HttpPost("{id:guid}/pets")]
    public async Task<IActionResult> AddPet(
        [FromServices] ICommandHandler<PetId, AddPetCommand> petHandler,
        [FromRoute] Guid id,
        [FromBody] AddPetRequest request,
        CancellationToken cancellationToken = default)
    {
        var addPetCommand = request.ToCommand(id);
        var result = await petHandler.HandleAsync(addPetCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var PetId = result.Value;

        var getUri = Request.Path + $"/{id}/Pets/{PetId}";

        return Created(getUri, PetId);
    }

    [Permission(Permissions.Pets.UPDATE)]
    [HttpPatch("{volunteerId:guid}/pet/{petId:guid}/serial-number")]
    public async Task<IActionResult> MovePet(
        [FromServices] ICommandHandler<PetId, MovePetCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] MovePetRequest request,
        CancellationToken cancellationToken = default)
    {
        var movePetCommand = request.ToCommand(volunteerId, petId);
        var result = await petHandler.HandleAsync(movePetCommand);

        if (result.IsFailure) return Error(result.Error);

        var resultPetId = result.Value;

        return Ok(resultPetId.Value);
    }

    [Permission(Permissions.Pets.UPDATE)]
    [HttpPut("{volunteerId:guid}/pet/{petId:guid}")]
    public async Task<IActionResult> UpdatePet(
        [FromServices] ICommandHandler<PetId, UpdatePetCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] UpdatePetRequest request,
        CancellationToken cancellationToken = default)
    {
        var updatePetCommand = request.ToCommand(volunteerId, petId);
        var result = await petHandler.HandleAsync(updatePetCommand);

        if (result.IsFailure) return Error(result.Error);

        var resultPetId = result.Value;

        return Ok(resultPetId.Value);
    }

    [Permission(Permissions.Pets.UPDATE)]
    [HttpPatch("{volunteerId:guid}/pet/{petId:guid}/status")]
    public async Task<IActionResult> UpdatePetStatus(
        [FromServices] ICommandHandler<PetId, UpdatePetStatusCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] UpdatePetStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var updatePetCommand = request.ToCommand(volunteerId, petId);
        var result = await petHandler.HandleAsync(updatePetCommand);

        if (result.IsFailure) return Error(result.Error);

        var resultPetId = result.Value;

        return Ok(resultPetId.Value);
    }

    [Permission(Permissions.Pets.DELETE)]
    [HttpDelete("{id:guid}/pets/{petid:guid}")]
    public async Task<IActionResult> DeletePet(
        [FromServices] ICommandHandler<PetId, DeletePetCommand> petHandler,
        [FromRoute] Guid id,
        [FromRoute] Guid petid,
        CancellationToken cancellationToken = default)
    {
        var deleteCommand = new DeletePetCommand(id, petid);
        var result = await petHandler.HandleAsync(deleteCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return Ok(volunteerId.Value);
    }

    [Permission(Permissions.Pets.UPDATE)]
    [HttpPost("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> UploadPetPhotos(
        [FromServices] ICommandHandler<PetId, UploadPetPhotosCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] UploadPetPhotosRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Files is null)
            return Error(ErrorHelper.General.ValueIsNull("FilesList"));

        await using var fileProcessor = new FormFileProcessor();
        var fileDTOs = fileProcessor.Process(request.Files);

        var uploadCommand = new UploadPetPhotosCommand(volunteerId, petId, fileDTOs);

        var result = await petHandler.HandleAsync(
            uploadCommand,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return CreatedBaseURI(result.Value);
    }

    [Permission(Permissions.Pets.UPDATE)]
    [HttpPatch("{volunteerId:guid}/pet/{petId:guid}/main-photo")]
    public async Task<IActionResult> SetMainPetPhoto(
        [FromServices] ICommandHandler<PetId, SetMainPetPhotoCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] SetMainPetPhotoRequest request,
        CancellationToken cancellationToken = default)
    {
        var mainPhotoCommand = new SetMainPetPhotoCommand(volunteerId, petId, request.PhotoPath);

        var result = await petHandler.HandleAsync(
            mainPhotoCommand,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Pets.UPDATE)]
    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> DeletePetPhotos(
        [FromServices] ICommandHandler<PetId, DeletePetPhotosCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] DeletePetPhotosRequest request,
        CancellationToken cancellationToken = default)
    {
        var deleteCommand = request.ToCommand(volunteerId, petId);

        var result = await petHandler.HandleAsync(
            deleteCommand,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Pets.READ)]
    [HttpGet("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> GetAllPetPhotos(
        [FromServices] GetPetPhotosHandler petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        //[FromRoute] GetFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var getCommand = new GetPetPhotosQuery(volunteerId, petId);

        var result = await petHandler.HandleAsync(
            getCommand,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }
}
