using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Models;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Interfaces;

public interface ITokenManager
{
    public string GenerateAccessToken(User user, Guid jti);

    public Result<AccessTokenData, ErrorList> GetDataFromToken(string accessToken);

    public Task<Result<RefreshSession, ErrorList>> GetRefreshSessionAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    public Task<Result<IEnumerable<RefreshSession>, ErrorList>> GetRefreshSessionsForUserAsync(
        Guid UserId,
        CancellationToken cancellationToken = default);

    public Task<Result<RefreshSession, ErrorList>> CreateRefreshSessionAsync(
        User user,
        CancellationToken cancellationToken = default);

    public Task<UnitResult<ErrorList>> DeleteRefreshSessionsAsync(
        IEnumerable<RefreshSession> refreshSessions,
        CancellationToken cancellationToken = default);

    public UnitResult<ErrorList> ValidateRefreshSession(
        AccessTokenData accessTokenData,
        RefreshSession refreshSession);
}
