namespace Template.Application.Features.Auth.Commands.Login;

public record LoginCommand(string UsernameOrEmail, string Password);
