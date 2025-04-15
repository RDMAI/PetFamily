namespace PetFamily.Files.Contracts.Requests;

public record GetFilesRequest(IEnumerable<Shared.Core.Files.FileInfo> Files);
