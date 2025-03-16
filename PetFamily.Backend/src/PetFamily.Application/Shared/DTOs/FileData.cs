namespace PetFamily.Application.Shared.DTOs;

public record FileData(Stream ContentStream, string Name, string BucketName);
