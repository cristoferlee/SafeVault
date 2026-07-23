using System.Data;
using Microsoft.Data.SqlClient;
using SafeVault.Web.Models;

namespace SafeVault.Web.Data;

public sealed class UserRepository
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
            SELECT UserID, Username, Email
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
            reader.GetString(reader.GetOrdinal("Email")));
    }
}