using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Shared.Interfaces;
public interface IFileProvider
{
    public Task<Result<string, Error>> UploadFileAsync(
        Stream stream,
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default);
    public Task<Result<string, Error>> DeleteFileAsync(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default);

    public Task<Result<string, Error>> GetFileAsync(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default);
}
