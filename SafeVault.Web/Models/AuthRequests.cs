namespace SafeVault.Web.Models;

public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password);

public sealed record LoginRequest(
    string Username,
    string Password);