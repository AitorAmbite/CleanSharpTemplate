namespace Template.Application.Features.Auth.Commands.Register;

using Microsoft.Extensions.Options;
using Template.Application.Abstractions;
using Template.Application.Configuration;
using Template.Contracts.Auth;
using Template.Domain;
using Template.Domain.Entities;
using Template.Domain.Repositories;

public class RegisterCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly TimeProvider _timeProvider;
    private readonly JwtConfig _jwtConfig;

    public RegisterCommandHandler(
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

    public async Task<TokenResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
        {
            throw new InvalidOperationException($"Username '{command.Username}' is already taken.");
        }

        if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            throw new InvalidOperationException($"Email '{command.Email}' is already registered.");
        }

        string passwordHash = _passwordHasher.Hash(command.Password);
        User user = User.Create(command.Username, command.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

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
}
