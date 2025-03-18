namespace PetFamily.API.Shared.Requests;

public record GetFilesRequest(IEnumerable<string> FilePaths);
