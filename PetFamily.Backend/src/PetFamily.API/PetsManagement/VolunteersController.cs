using Microsoft.AspNetCore.Mvc;
using PetFamily.API.PetsManagement.Pets.Extensions;
using PetFamily.API.PetsManagement.Pets.Requests;
using PetFamily.API.PetsManagement.Volunteers.Extensions;
using PetFamily.API.PetsManagement.Volunteers.Requests;
using PetFamily.API.Shared;
using PetFamily.API.Shared.Processors;
using PetFamily.API.Shared.Requests;
using PetFamily.Application.PetsManagement.Pets.AddPet;
using PetFamily.Application.PetsManagement.Pets.MovePet;
using PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.DeleteVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.UpdateMainInfo;
using PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
using PetFamily.Application.PetsManagement.Volunteers.UpdateSocialNetworks;
using PetFamily.Domain.Helpers;

namespace PetFamily.API.PetsManagement;

public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateVolunteerHandler volunteerHandler,
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
        [FromServices] UpdateMainInfoHandler volunteerHandler,
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
        [FromServices] UpdateSocialNetworksHandler volunteerHandler,
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
        [FromServices] UpdateRequisitesHandler volunteerHandler,
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
        [FromServices] DeleteVolunteerHandler volunteerHandler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleteCommand = new DeleteVolunteerCommand(id);
        var result = await volunteerHandler.HandleAsync(deleteCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return Ok(volunteerId.Value);
    }

    [HttpPost("{id:guid}/Pets")]
    public async Task<IActionResult> AddPet(
        [FromServices] AddPetHandler petHandler,
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

    [HttpPatch("{volunteerId:guid}/Pet/{petId:guid}/SerialNumber")]
    public async Task<IActionResult> MovePet(
        [FromServices] MovePetHandler petHandler,
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

    [HttpPost("{volunteerId:guid}/Pet/{petId:guid}/Photos")]
    public async Task<IActionResult> UploadPetPhotos(
        [FromServices] UploadPetPhotosHandler petHandler,
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

    [HttpDelete("{volunteerId:guid}/Pet/{petId:guid}/Photos")]
    public async Task<IActionResult> DeletePetPhotos(
        [FromServices] DeletePetPhotosHandler petHandler,
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

    [HttpGet("{volunteerId:guid}/Pet/{petId:guid}/Photos")]
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
