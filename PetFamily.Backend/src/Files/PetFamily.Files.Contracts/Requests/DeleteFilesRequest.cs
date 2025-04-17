using PetFamily.Shared.Core.Files;

namespace PetFamily.Files.Contracts.Requests;

public record DeleteFilesRequest(IEnumerable<FileInfoDTO> Files);
