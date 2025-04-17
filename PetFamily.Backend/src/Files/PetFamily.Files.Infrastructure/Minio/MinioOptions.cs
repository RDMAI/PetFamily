namespace PetFamily.Files.Infrastructure.Minio;
public class MinioOptions
{
    public required string Endpoint { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required bool WithSSL { get; set; }
}
