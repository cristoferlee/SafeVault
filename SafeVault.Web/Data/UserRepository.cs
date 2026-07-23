using System.Data;
using Microsoft.Data.SqlClient;
using SafeVault.Web.Models;

namespace SafeVault.Web.Data;

public sealed class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("SafeVaultDatabase")
            ?? throw new InvalidOperationException(
                "The database connection string is missing.");
    }

    public async Task<UserRecord?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        const string query = """
            SELECT UserID, Username, Email, PasswordHash, Role
            FROM Users
            WHERE Username = @Username;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(query, connection);

        command.Parameters.Add(
            new SqlParameter("@Username", SqlDbType.VarChar, 100)
            {
                Value = username
            });

        await connection.OpenAsync(cancellationToken);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserRecord(
            reader.GetInt32(reader.GetOrdinal("UserID")),
            reader.GetString(reader.GetOrdinal("Username")),
            reader.GetString(reader.GetOrdinal("Email")),
            reader.GetString(reader.GetOrdinal("PasswordHash")),
            reader.GetString(reader.GetOrdinal("Role")));
    }

    public async Task CreateAsync(
        string username,
        string email,
        string passwordHash,
        string role,
        CancellationToken cancellationToken = default)
    {
        const string query = """
            INSERT INTO Users
                (Username, Email, PasswordHash, Role)
            VALUES
                (@Username, @Email, @PasswordHash, @Role);
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await using var command =
            new SqlCommand(query, connection);

        command.Parameters.Add(
            new SqlParameter("@Username", SqlDbType.VarChar, 100)
            {
                Value = username
            });

        command.Parameters.Add(
            new SqlParameter("@Email", SqlDbType.VarChar, 100)
            {
                Value = email
            });

        command.Parameters.Add(
            new SqlParameter("@PasswordHash", SqlDbType.VarChar, 255)
            {
                Value = passwordHash
            });

        command.Parameters.Add(
            new SqlParameter("@Role", SqlDbType.VarChar, 20)
            {
                Value = role
            });

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}