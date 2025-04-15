using Microsoft.AspNetCore.Http;
using PetFamily.Shared.Core.Files;

namespace PetFamily.Shared.Framework.Processors;

public class FormFileProcessor : IAsyncDisposable
{
    private readonly List<FileDTO> _fileDtos = [];

    public List<FileDTO> Process(IFormFileCollection files)
    {
        if (files is null)
            throw new ArgumentNullException("Files list is empty.");

        foreach (var file in files)
        {
            var stream = file.OpenReadStream();

            var fileDto = new FileDTO(stream, file.FileName);

            _fileDtos.Add(fileDto);
        }
        return _fileDtos;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var file in _fileDtos)
        {
            await file.ContentStream.DisposeAsync();
        }
    }
}
