using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Infrastructure.Providers;
public class MinioProvider : IFileProvider
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioProvider> _logger;

    public const int MAX_PARALLEL_UPLOADS_THREADS = 3;
    public const int MAX_PARALLEL_DELETE_THREADS = 3;
    public const int MAX_PARALLEL_GET_THREADS = 6;

    public MinioProvider(IMinioClient client, ILogger<MinioProvider> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> UploadFilesAsync(
        IEnumerable<FileData> files,
        CancellationToken cancellationToken = default)
    {
        var sem = new SemaphoreSlim(MAX_PARALLEL_UPLOADS_THREADS);

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

                return ErrorHelper.Files.UploadFailure().ToErrorList();
            }
            finally
            {
                sem.Release();
            }
        }
        await Task.WhenAll(tasks);

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> DeleteFilesAsync(
        IEnumerable<string> filePaths,
        string bucketName,
        CancellationToken cancellationToken = default)
    {
        var sem = new SemaphoreSlim(MAX_PARALLEL_DELETE_THREADS);

        List<Task> tasks = [];
        foreach (var path in filePaths)
        {
            await sem.WaitAsync(cancellationToken);
            try
            {
                var args = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(path);

                // to run task in separate thread from system threadpool
                var task = Task.Run(async () =>
                {
                    await _client.RemoveObjectAsync(args, cancellationToken);
                }, cancellationToken);

                tasks.Add(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Cannot delete file from minio; Path: {path}; Bucket: {bucket}",
                    path,
                    bucketName);

                return ErrorHelper.Files.DeleteFailure().ToErrorList();
            }
            finally
            {
                sem.Release();
            }
        }
        await Task.WhenAll(tasks);

        return UnitResult.Success<ErrorList>();
    }

    public async Task<Result<IEnumerable<string>, ErrorList>> GetFilesAsync(
        string bucketName,
        IEnumerable<FileVO> files,
        CancellationToken cancellationToken = default)
    {
        var sem = new SemaphoreSlim(MAX_PARALLEL_GET_THREADS);

        List<Task> tasks = [];
        List<string> presignedURLS = [];

        foreach (var file in files)
        {
            await sem.WaitAsync(cancellationToken);
            try
            {
                var args = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(file.PathToStorage)
                    .WithExpiry(60 * 60 * 24);

                // to run task in separate thread from system threadpool
                var task = Task.Run(async () =>
                {
                    var result = await _client.PresignedGetObjectAsync(args);

                    presignedURLS.Add(result);
                }, cancellationToken);

                tasks.Add(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Cannot get file from minio; Name: {name}; Bucket: {bucket}",
                    file.Name,
                    bucketName);

                return ErrorHelper.Files.GetFailure().ToErrorList();
            }
            finally
            {
                sem.Release();
            }
        }

        await Task.WhenAll(tasks);

        return presignedURLS;
    }

    public async Task CreateRequiredBuckets(CancellationToken cancellationToken = default)
    {
        // getting list of required buckets
        var bucketNamesList = typeof(Constants.BucketNames)
                .GetFields()
                .Where(f => f.IsLiteral)  // only consts
                .Select(f => f.GetRawConstantValue()!.ToString()!)
                .ToList();

        foreach (var bucketName in bucketNamesList)
        {
            // create bucket if it does not exist
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            if (await _client.BucketExistsAsync(bucketExistArgs, cancellationToken) == false)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);

                await _client.MakeBucketAsync(makeBucketArgs, cancellationToken);

                _logger.LogInformation("Bucket {name} created", bucketName);
            }
        }
    }
}
