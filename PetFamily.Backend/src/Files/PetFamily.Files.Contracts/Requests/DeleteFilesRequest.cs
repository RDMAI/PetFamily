namespace PetFamily.Files.Contracts.Requests;

public record DeleteFilesRequest(IEnumerable<Shared.Core.Files.FileInfo> Files);
