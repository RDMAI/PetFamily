using CSharpFunctionalExtensions;
using PetFamily.Files.Application;
using PetFamily.Files.Contracts;
using PetFamily.Files.Contracts.Requests;
using PetFamily.Shared.Kernel;

namespace PetFamily.Files.Presentation;

public class FileContract : IFileContract
{
    private readonly IFileProvider _provider;

    public FileContract(IFileProvider provider)
    {
        _provider = provider;
    }

    public Task<UnitResult<ErrorList>> DeleteFilesAsync(
        DeleteFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var files = request.Files;
        return _provider.DeleteFilesAsync(files, cancellationToken);
    }

    public Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        GetFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var files = request.Files;
        return _provider.GetFilesAsync(files, cancellationToken);
    }

    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        UploadFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var files = request.Files;
        return _provider.UploadFilesAsync(files, cancellationToken);
    }
}
