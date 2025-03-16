using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.Shared.Processors;

public class FormFileProcessor : IAsyncDisposable
{
    private readonly List<FileDTO> _fileDtos = [];

    public List<FileDTO> Process(IFormFileCollection files)
    {
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
