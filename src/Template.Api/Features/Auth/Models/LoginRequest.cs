namespace Template.Api.Features.Auth.Models;

public record LoginRequest(string UsernameOrEmail, string Password);
