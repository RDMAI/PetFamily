namespace PetFamily.API.Shared.Requests;

public record DeleteFilesRequest(IEnumerable<string> FilePaths);
