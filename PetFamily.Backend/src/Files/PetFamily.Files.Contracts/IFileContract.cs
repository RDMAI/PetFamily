using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Files.Contracts;

public interface IFileContract
{
    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<Shared.Core.Files.FileData> files,
        CancellationToken cancellationToken = default);

    Task<UnitResult<ErrorList>> DeleteFilesAsync(
        IEnumerable<Shared.Core.Files.FileInfo> files,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        IEnumerable<Shared.Core.Files.FileInfo> files,
        CancellationToken cancellationToken = default);
}
