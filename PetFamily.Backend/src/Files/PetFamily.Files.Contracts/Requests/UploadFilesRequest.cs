namespace PetFamily.Files.Contracts.Requests;

public record UploadFilesRequest(IEnumerable<Shared.Core.Files.FileData> Files);
