namespace Template.Application.Features.Auth.Commands.RefreshToken;

using Microsoft.Extensions.Options;
using Template.Application.Abstractions;
using Template.Application.Configuration;
using Template.Contracts.Auth;
using Template.Domain;
using Template.Domain.Entities;
using Template.Domain.Repositories;

public class RefreshTokenCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly TimeProvider _timeProvider;
    private readonly JwtConfig _jwtConfig;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        TimeProvider timeProvider,
        IOptions<JwtConfig> jwtConfig)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _timeProvider = timeProvider;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<TokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        RefreshToken? existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);

        if (existingRefreshToken is null || !existingRefreshToken.IsActive)
        {
            throw new InvalidOperationException("Invalid refresh token.");
        }

        User? user = await _userRepository.FindByIdAsync(existingRefreshToken.UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        string accessToken = _jwtTokenService.GenerateAccessToken(user);
        string newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        existingRefreshToken.Revoke(newRefreshTokenValue);

        RefreshToken newRefreshToken = RefreshToken.Create(
            user.Id,
            newRefreshTokenValue,
            _timeProvider.GetUtcNow().UtcDateTime.AddDays(_jwtConfig.RefreshTokenExpirationDays));

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse(accessToken, newRefreshTokenValue, _jwtConfig.AccessTokenExpirationMinutes * 60, "Bearer");
    }
}
