using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Application.Models;
using PetFamily.Accounts.Domain;
using PetFamily.Accounts.Infrastructure.Identity.Options;
using PetFamily.Shared.Kernel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetFamily.Accounts.Infrastructure.Identity.Managers;

public class TokenManager : ITokenManager
{
    private readonly JWTOptions _options;
    private readonly AccountDBContext _accountContext;

    public TokenManager(
        IOptions<JWTOptions> options,
        AccountDBContext accountContext)
    {
        _options = options.Value;
        _accountContext = accountContext;
    }

    public string GenerateAccessToken(User user, Guid jti)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),
            ]),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_options.ExpiredMinutesTime),
            SigningCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Key)),
                algorithm: SecurityAlgorithms.HmacSha256Signature
                )
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public Result<AccessTokenData, ErrorList> GetDataFromToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = GetTokenValidationParameters(_options);
        validationParameters.ValidateLifetime = false;

        var claimsPrincipal = tokenHandler.ValidateToken(
            accessToken,
            validationParameters,
            out _);

        var errors = new List<Error>();
        var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            errors.Add(ErrorHelper.Authentication.InvalidAccessToken("Email is invalid"));

        var userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (false == Guid.TryParse(userIdString, out var userId))
            errors.Add(ErrorHelper.Authentication.InvalidAccessToken("UserId is invalid"));

        var jtiString = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (false == Guid.TryParse(jtiString, out var jti))
            errors.Add(ErrorHelper.Authentication.InvalidAccessToken("Jti is invalid"));

        if (errors.Count > 0)
            return new ErrorList(errors);

        return new AccessTokenData(email!, userId, jti);
    }

    public async Task<Result<RefreshSession, ErrorList>> GetRefreshSessionAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken) || false == Guid.TryParse(refreshToken, out var refreshTokenAsGuid))
            return ErrorHelper.Authentication.InvalidRefreshToken().ToErrorList();

        var refreshSession = await _accountContext.RefreshSessions.FirstOrDefaultAsync(
            r => r.RefreshToken == refreshTokenAsGuid,
            cancellationToken);
        if (refreshSession is null)
            return ErrorHelper.Authentication.InvalidRefreshToken().ToErrorList();

        return refreshSession;
    }

    public async Task<Result<IEnumerable<RefreshSession>, ErrorList>> GetRefreshSessionsForUserAsync(
        Guid UserId,
        CancellationToken cancellationToken = default)
    {
        if (Guid.Empty == UserId)
            return ErrorHelper.Authentication.UserNotFound().ToErrorList();

        return await _accountContext.RefreshSessions
            .Where(r => r.UserId == UserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<RefreshSession, ErrorList>> CreateRefreshSessionAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        var newRefreshSession = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDaysTime),
            RefreshToken = Guid.NewGuid(),
            Jti = Guid.NewGuid(),
        };

        _accountContext.RefreshSessions.Add(newRefreshSession);
        await _accountContext.SaveChangesAsync(cancellationToken);

        return newRefreshSession;
    }

    public async Task<UnitResult<ErrorList>> DeleteRefreshSessionsAsync(
        IEnumerable<RefreshSession> refreshSessions,
        CancellationToken cancellationToken = default)
    {
        _accountContext.RefreshSessions.RemoveRange(refreshSessions);
        await _accountContext.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<ErrorList>();
    }

    public UnitResult<ErrorList> ValidateRefreshSession(
        AccessTokenData accessTokenData,
        RefreshSession refreshSession)
    {
        if (refreshSession.UserId != accessTokenData.UserId)
            return ErrorHelper.Authentication.InvalidRefreshToken("UserId is invalid").ToErrorList();

        if (refreshSession.ExpiresAt <= DateTime.UtcNow)
            return ErrorHelper.Authentication.InvalidRefreshToken("Refresh token has expired").ToErrorList();

        return UnitResult.Success<ErrorList>();
    }

    public static TokenValidationParameters GetTokenValidationParameters(JWTOptions jWTOptions)
    {
        return new TokenValidationParameters
        {
            ValidIssuer = jWTOptions.Issuer,
            ValidateIssuer = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jWTOptions.Key)),
            ValidateIssuerSigningKey = true,
            ValidAudience = jWTOptions.Audience,
            ValidateAudience = true,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };
    }
}
