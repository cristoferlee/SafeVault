using SafeVault.Web.Data;
using SafeVault.Web.Models;

namespace SafeVault.Tests;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly List<UserRecord> _users = [];

    public void AddUser(UserRecord user)
    {
        _users.Add(user);
    }

    public Task<UserRecord?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        UserRecord? user = _users.FirstOrDefault(
            candidate => string.Equals(
                candidate.Username,
                username,
                StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task CreateAsync(
        string username,
        string email,
        string passwordHash,
        string role,
        CancellationToken cancellationToken = default)
    {
        int userId = _users.Count + 1;

        _users.Add(
            new UserRecord(
                userId,
                username,
                email,
                passwordHash,
                role));

        return Task.CompletedTask;
    }
}