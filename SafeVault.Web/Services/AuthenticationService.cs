using SafeVault.Web.Data;
using SafeVault.Web.Models;

namespace SafeVault.Web.Services;

public sealed class AuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<UserRecord?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        UserRecord? user =
            await _userRepository.GetByUsernameAsync(
                username,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        bool passwordIsValid =
            BCrypt.Net.BCrypt.Verify(
                password,
                user.PasswordHash);

        return passwordIsValid ? user : null;
    }
}