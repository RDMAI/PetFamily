using PetFamily.Shared.Core.Files;
namespace PetFamily.Files.Contracts.Requests;

public record GetFilesRequest(IEnumerable<FileInfoDTO> Files);
