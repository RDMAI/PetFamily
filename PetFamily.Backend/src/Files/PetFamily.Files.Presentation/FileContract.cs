using CSharpFunctionalExtensions;
using PetFamily.Files.Application;
using PetFamily.Files.Contracts;
using PetFamily.Shared.Core.Files;
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
        IEnumerable<Shared.Core.Files.FileInfo> files,
        CancellationToken cancellationToken = default)
    {
        return _provider.DeleteFilesAsync(files, cancellationToken);
    }

    public Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        IEnumerable<Shared.Core.Files.FileInfo> files,
        CancellationToken cancellationToken = default)
    {
        return _provider.GetFilesAsync(files, cancellationToken);
    }

    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<FileData> files,
        CancellationToken cancellationToken = default)
    {
        return _provider.UploadFilesAsync(files, cancellationToken);
    }
}
