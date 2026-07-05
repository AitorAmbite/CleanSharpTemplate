namespace Template.Application.Features.Auth.Commands.Login;

using Microsoft.Extensions.Options;
using Template.Application.Abstractions;
using Template.Application.Configuration;
using Template.Contracts.Auth;
using Template.Domain;
using Template.Domain.Entities;
using Template.Domain.Repositories;

public class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly TimeProvider _timeProvider;
    private readonly JwtConfig _jwtConfig;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        TimeProvider timeProvider,
        IOptions<JwtConfig> jwtConfig)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _timeProvider = timeProvider;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<TokenResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await FindUserAsync(command.UsernameOrEmail, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        string accessToken = _jwtTokenService.GenerateAccessToken(user);
        string refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        RefreshToken refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenValue,
            _timeProvider.GetUtcNow().UtcDateTime.AddDays(_jwtConfig.RefreshTokenExpirationDays));

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse(accessToken, refreshTokenValue, _jwtConfig.AccessTokenExpirationMinutes * 60, "Bearer");
    }

    private async Task<User?> FindUserAsync(string usernameOrEmail, CancellationToken cancellationToken)
    {
        if (usernameOrEmail.Contains('@', StringComparison.OrdinalIgnoreCase))
        {
            return await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken);
        }

        return await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);
    }
}
