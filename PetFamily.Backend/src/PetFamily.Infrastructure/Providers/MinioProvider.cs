using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

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
                    .WithBucket(file.Info.BucketName)
                    .WithObject(file.Info.FilePath)
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
                    file.Info.FilePath,
                    file.Info.BucketName);

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
        IEnumerable<FileInfoDTO> files,
        CancellationToken cancellationToken = default)
    {
        var sem = new SemaphoreSlim(MAX_PARALLEL_DELETE_THREADS);

        List<Task> tasks = [];
        foreach (var file in files)
        {
            await sem.WaitAsync(cancellationToken);
            try
            {
                var existanceCheck = await _checkFileExistanceAsync(file, cancellationToken);
                if (existanceCheck.IsFailure)
                    return existanceCheck.Error.ToErrorList();

                var args = new RemoveObjectArgs()
                    .WithBucket(file.BucketName)
                    .WithObject(file.FilePath);

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
                    file.FilePath,
                    file.BucketName);

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
        IEnumerable<FileInfoDTO> files,
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
                var existanceCheck = await _checkFileExistanceAsync(file, cancellationToken);
                if (existanceCheck.IsFailure)
                    return existanceCheck.Error.ToErrorList();

                var args = new PresignedGetObjectArgs()
                    .WithBucket(file.BucketName)
                    .WithObject(file.FilePath)
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
                    file.FilePath,
                    file.BucketName);

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

    private async Task<UnitResult<Error>> _checkFileExistanceAsync(
        FileInfoDTO file,
        CancellationToken cancellationToken = default)
    {
        var statArgs = new StatObjectArgs()
                    .WithBucket(file.BucketName)
                    .WithObject(file.FilePath);
        var res = await _client.StatObjectAsync(statArgs, cancellationToken);
        if (res.ContentType is null)
            return ErrorHelper.Files.DeleteFailure("File not found");

        return UnitResult.Success<Error>();
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
