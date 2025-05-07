namespace PetFamily.Accounts.Application.Models;

public record AccessTokenData(string Email, Guid UserId, Guid Jti);
