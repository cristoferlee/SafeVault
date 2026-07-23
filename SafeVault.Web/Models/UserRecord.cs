namespace SafeVault.Web.Models;

public sealed record UserRecord(
    int UserId,
    string Username,
    string Email);