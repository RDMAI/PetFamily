using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.RefreshTokens;

public record RefreshTokensCommand(
    string AccessToken,
    string RefreshToken) : ICommand;
