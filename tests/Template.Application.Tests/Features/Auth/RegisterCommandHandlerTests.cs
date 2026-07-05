namespace Template.Application.Tests.Features.Auth;

using Microsoft.Extensions.Options;
using NSubstitute;
using Template.Application.Abstractions;
using Template.Application.Configuration;
using Template.Application.Features.Auth.Commands.Register;
using Template.Application.Tests.Fakes;
using Template.Contracts.Auth;
using Template.Domain;
using Template.Domain.Entities;
using Template.Domain.Repositories;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly FakeTimeProvider _timeProvider;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
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

        _handler = new RegisterCommandHandler(
            _userRepository,
            _refreshTokenRepository,
            _unitOfWork,
            _passwordHasher,
            _jwtTokenService,
            _timeProvider,
            jwtConfig);
    }

    [Fact]
    public async Task Handle_Creates_User_And_Returns_Tokens()
    {
        var command = new RegisterCommand("testuser", "test@example.com", "Password123!");
        _userRepository.ExistsByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.Hash(command.Password).Returns("hashedPassword");
        _jwtTokenService.GenerateAccessToken(Arg.Any<User>()).Returns("accessToken");
        _jwtTokenService.GenerateRefreshToken().Returns("refreshToken");

        TokenResponse response = await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Username == command.Username &&
            u.Email == command.Email &&
            u.PasswordHash == "hashedPassword"), Arg.Any<CancellationToken>());

        await _refreshTokenRepository.Received(1).AddAsync(Arg.Is<RefreshToken>(r =>
            r.Token == "refreshToken" &&
            r.UserId != Guid.Empty), Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        Assert.Equal("accessToken", response.AccessToken);
        Assert.Equal("refreshToken", response.RefreshToken);
        Assert.Equal("Bearer", response.TokenType);
    }

    [Fact]
    public async Task Handle_With_Duplicate_Username_Throws_InvalidOperationException()
    {
        var command = new RegisterCommand("existinguser", "test@example.com", "Password123!");
        _userRepository.ExistsByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns(true);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("Username", exception.Message);
    }

    [Fact]
    public async Task Handle_With_Duplicate_Email_Throws_InvalidOperationException()
    {
        var command = new RegisterCommand("testuser", "existing@example.com", "Password123!");
        _userRepository.ExistsByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(true);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("Email", exception.Message);
    }
}
