using SafeVault.Web.Models;

namespace SafeVault.Web.Data;

public interface IUserRepository
{
    Task<UserRecord?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        string username,
        string email,
        string passwordHash,
        string role,
        CancellationToken cancellationToken = default);
}