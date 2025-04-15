using CSharpFunctionalExtensions;
using PetFamily.Shared.Core.Files;
using PetFamily.Shared.Kernel;

namespace PetFamily.Files.Application;

public interface IFileProvider
{
    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<FileData> files,
        CancellationToken cancellationToken = default);

    public Task CreateRequiredBuckets(
        CancellationToken cancellationToken = default);

    Task<UnitResult<ErrorList>> DeleteFilesAsync(
        IEnumerable<Shared.Core.Files.FileInfo> files,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        IEnumerable<Shared.Core.Files.FileInfo> files,
        CancellationToken cancellationToken = default);
}
