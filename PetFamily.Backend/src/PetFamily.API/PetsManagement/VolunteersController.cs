﻿using Microsoft.AspNetCore.Mvc;
using PetFamily.API.PetsManagement.Pets.Requests;
using PetFamily.API.PetsManagement.Volunteers.Requests;
using PetFamily.API.Shared;
using PetFamily.API.Shared.Processors;
using PetFamily.API.Shared.Requests;
using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;
using PetFamily.Application.PetsManagement.Pets.Commands.MovePet;
using PetFamily.Application.PetsManagement.Pets.Commands.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.Commands.DeleteVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateRequisites;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;

namespace PetFamily.API.PetsManagement;

public class VolunteersController : ApplicationController
{
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

    [HttpPost("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> UploadPetPhotos(
        [FromServices] ICommandHandler<PetId, UploadPetPhotosCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] UploadFilesRequest request,
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

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> DeletePetPhotos(
        [FromServices] ICommandHandler<PetId, DeletePetPhotosCommand> petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] DeleteFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var deleteCommand = new DeletePetPhotosCommand(
            volunteerId,
            petId,
            request.FilePaths);

        var result = await petHandler.HandleAsync(
            deleteCommand,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<IActionResult> GetAllPetPhotos(
        [FromServices] GetPetPhotosHandler petHandler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        //[FromRoute] GetFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var getCommand = new GetPetPhotosCommand(volunteerId, petId);

        var result = await petHandler.HandleAsync(
            getCommand,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }
}
