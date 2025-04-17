using CSharpFunctionalExtensions;
using PetFamily.Files.Contracts.Requests;
using PetFamily.Shared.Kernel;

namespace PetFamily.Files.Contracts;

public interface IFileContract
{
    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        UploadFilesRequest request,
        CancellationToken cancellationToken = default);

    Task<UnitResult<ErrorList>> DeleteFilesAsync(
        DeleteFilesRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        GetFilesRequest request,
        CancellationToken cancellationToken = default);
}
