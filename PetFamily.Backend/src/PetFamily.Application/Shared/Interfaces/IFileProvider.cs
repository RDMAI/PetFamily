using CSharpFunctionalExtensions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Shared.Interfaces;
public interface IFileProvider
{
    public Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<FileData> files,
        CancellationToken cancellationToken = default);

    //public Task<Result<string, Error>> DeleteFileAsync(
    //    string bucketName,
    //    string fileName,
    //    CancellationToken cancellationToken = default);

    //public Task<Result<string, Error>> GetFileAsync(
    //    string bucketName,
    //    string fileName,
    //    CancellationToken cancellationToken = default);
}
