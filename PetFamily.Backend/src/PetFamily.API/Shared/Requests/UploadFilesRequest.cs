namespace PetFamily.API.Shared.Requests;

public record UploadFilesRequest(
    IFormFileCollection Files);
