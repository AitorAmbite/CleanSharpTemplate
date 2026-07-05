namespace Template.Application.Tests.Features.Auth;

using Microsoft.Extensions.Options;
using NSubstitute;
using Template.Application.Abstractions;
using Template.Application.Configuration;
using Template.Application.Features.Auth.Commands.Login;
using Template.Application.Tests.Fakes;
using Template.Contracts.Auth;
using Template.Domain;
using Template.Domain.Entities;
using Template.Domain.Repositories;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly FakeTimeProvider _timeProvider;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenService = Substitute.For<IJwtTokenService>();
        _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var jwtConfig = Options.Create(new JwtConfig
        {
            Issuer = "Template",
            Audience = "Template",
            Secret = "test-secret-must-be-at-least-32-bytes-long!",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7,
        });

        _handler = new LoginCommandHandler(
            _userRepository,
            _refreshTokenRepository,
            _unitOfWork,
            _passwordHasher,
            _jwtTokenService,
            _timeProvider,
            jwtConfig);
    }

    [Fact]
    public async Task Handle_With_Valid_Credentials_Returns_Tokens()
    {
        var user = User.Create("testuser", "test@example.com", "hashedPassword");
        var command = new LoginCommand("testuser", "Password123!");
        _userRepository.GetByUsernameAsync(command.UsernameOrEmail, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
        _jwtTokenService.GenerateAccessToken(user).Returns("accessToken");
        _jwtTokenService.GenerateRefreshToken().Returns("refreshToken");

        TokenResponse response = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("accessToken", response.AccessToken);
        Assert.Equal("refreshToken", response.RefreshToken);
        Assert.Equal("Bearer", response.TokenType);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_With_Invalid_User_Throws_InvalidOperationException()
    {
        var command = new LoginCommand("unknown", "Password123!");
        _userRepository.GetByUsernameAsync(command.UsernameOrEmail, Arg.Any<CancellationToken>()).Returns((User?)null);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Invalid credentials.", exception.Message);
    }

    [Fact]
    public async Task Handle_With_Invalid_Password_Throws_InvalidOperationException()
    {
        var user = User.Create("testuser", "test@example.com", "hashedPassword");
        var command = new LoginCommand("testuser", "WrongPassword");
        _userRepository.GetByUsernameAsync(command.UsernameOrEmail, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Invalid credentials.", exception.Message);
    }
}
