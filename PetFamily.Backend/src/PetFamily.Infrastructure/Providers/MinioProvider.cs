using CSharpFunctionalExtensions;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Shared;
using System.IO;
using System.Security.AccessControl;

namespace PetFamily.Infrastructure.Providers;
public class MinioProvider : IFileProvider
{
    private readonly IMinioClient _client;

    public MinioProvider(IMinioClient client)
    {
        _client = client;
    }

    public async Task<Result<string, Error>> UploadFileAsync(
        Stream stream,
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        // create bucket if it does not exist
        var bucketExistArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        if (await _client.BucketExistsAsync(bucketExistArgs, cancellationToken) == false)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);

            await _client.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length);

        var result = await _client.PutObjectAsync(args, cancellationToken);

        return result.ObjectName;
    }

    public async Task<Result<string, Error>> DeleteFileAsync(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName);

        await _client.RemoveObjectAsync(args, cancellationToken);

        return fileName;
    }

    public async Task<Result<string, Error>> GetFileAsync(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithExpiry(60*60*24);

        var result = await _client.PresignedGetObjectAsync(args);

        return result;
    }
}
