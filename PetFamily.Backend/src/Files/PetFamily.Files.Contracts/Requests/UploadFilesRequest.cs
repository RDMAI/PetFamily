using PetFamily.Shared.Core.Files;

namespace PetFamily.Files.Contracts.Requests;

public record UploadFilesRequest(IEnumerable<FileDataDTO> Files);
