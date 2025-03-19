using CSharpFunctionalExtensions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Shared.Interfaces;

public interface IFileProvider
{
    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<FileData> files,
        CancellationToken cancellationToken = default);

    public Task CreateRequiredBuckets(
        CancellationToken cancellationToken = default);

    Task<UnitResult<ErrorList>> DeleteFilesAsync(
        IEnumerable<FileInfoDTO> files,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        IEnumerable<FileInfoDTO> files,
        CancellationToken cancellationToken = default);
}
