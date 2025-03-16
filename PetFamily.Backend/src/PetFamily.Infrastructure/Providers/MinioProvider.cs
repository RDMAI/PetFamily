using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.Providers;
public class MinioProvider : IFileProvider
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioProvider> _logger;

    public MinioProvider(IMinioClient client, ILogger<MinioProvider> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<FileData> files,
        CancellationToken cancellationToken = default)
    {
        //// create bucket if it does not exist
        //var bucketExistArgs = new BucketExistsArgs()
        //    .WithBucket(bucketName);
        //if (await _client.BucketExistsAsync(bucketExistArgs, cancellationToken) == false)
        //{
        //    var makeBucketArgs = new MakeBucketArgs()
        //        .WithBucket(bucketName);

        //    await _client.MakeBucketAsync(makeBucketArgs, cancellationToken);
        //}

        var sem = new SemaphoreSlim(3);
        List<Task> tasks = [];
        foreach (var file in files)
        {
            await sem.WaitAsync(cancellationToken);
            try
            {
                var args = new PutObjectArgs()
                    .WithBucket(file.BucketName)
                    .WithObject(file.Name)
                    .WithStreamData(file.ContentStream)
                    .WithObjectSize(file.ContentStream.Length);

                // to run task in separate thread from system threadpool
                var task = Task.Run(async () =>
                {
                    var result = await _client.PutObjectAsync(args, cancellationToken);
                }, cancellationToken);

                tasks.Add(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Cannot upload file to minio; Name: {name}; Bucket: {bucket}",
                    file.Name,
                    file.BucketName);
            }
            finally
            {
                sem.Release();
            }
        }

        await Task.WhenAll(tasks);

        return UnitResult.Success<ErrorList>();
    }

    //public async Task<Result<string, Error>> DeleteFileAsync(
    //    string bucketName,
    //    string fileName,
    //    CancellationToken cancellationToken = default)
    //{
    //    var args = new RemoveObjectArgs()
    //        .WithBucket(bucketName)
    //        .WithObject(fileName);

    //    await _client.RemoveObjectAsync(args, cancellationToken);

    //    return fileName;
    //}

    //public async Task<Result<string, Error>> GetFileAsync(
    //    string bucketName,
    //    string fileName,
    //    CancellationToken cancellationToken = default)
    //{
    //    var args = new PresignedGetObjectArgs()
    //        .WithBucket(bucketName)
    //        .WithObject(fileName)
    //        .WithExpiry(60*60*24);

    //    var result = await _client.PresignedGetObjectAsync(args);

    //    return result;
    //}

    //public async Task<UnitResult<Error>> CreateBucketsIfNotExistAsync(
    //    IEnumerable<string> bucketName,
    //    CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        var bucketExistArgs = new BucketExistsArgs()
    //        .WithBucket(bucketName);
    //        if (await _client.BucketExistsAsync(bucketExistArgs, cancellationToken) == false)
    //        {
    //            var makeBucketArgs = new MakeBucketArgs()
    //                .WithBucket(bucketName);

    //            await _client.MakeBucketAsync(makeBucketArgs, cancellationToken);
    //        }

    //        return UnitResult.Success<Error>();
    //    }
    //    catch (Exception ex)
    //    {
    //        return Error.Failure("Minio.failure", )
    //    }
    //}
}
