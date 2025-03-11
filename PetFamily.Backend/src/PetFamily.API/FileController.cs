using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Shared;
using PetFamily.Application.Shared.Interfaces;

namespace PetFamily.API;

public class FileController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] IFileProvider fileProvider,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await using Stream fileStream = file.OpenReadStream();
        var fileName = file.FileName;
        var bucketName = "photos";

        var result = await fileProvider.UploadFileAsync(
            fileStream,
            bucketName,
            fileName,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return CreatedBaseURI(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] IFileProvider fileProvider,
        [FromQuery] string FileName,
        CancellationToken cancellationToken = default)
    {
        var bucketName = "photos";

        var result = await fileProvider.GetFileAsync(
            bucketName,
            FileName,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromServices] IFileProvider fileProvider,
        [FromQuery] string FileName,
        CancellationToken cancellationToken = default)
    {
        var bucketName = "photos";

        var result = await fileProvider.DeleteFileAsync(
            bucketName,
            FileName,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }
}
