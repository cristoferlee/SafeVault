using NUnit.Framework;
using SafeVault.Web.Models;

using AuthenticationService =
    SafeVault.Web.Services.AuthenticationService;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthentication
{
    [Test]
    public async Task ValidCredentialsAuthenticateUser()
    {
        var repository = new FakeUserRepository();
        var authenticationService =
            new AuthenticationService(repository);

        string passwordHash =
            authenticationService.HashPassword(
                "SecurePassword123!");

        repository.AddUser(
            new UserRecord(
                1,
                "admin",
                "admin@safevault.test",
                passwordHash,
                "Admin"));

        UserRecord? result =
            await authenticationService.AuthenticateAsync(
                "admin",
                "SecurePassword123!");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Role, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task InvalidPasswordDoesNotAuthenticateUser()
    {
        var repository = new FakeUserRepository();
        var authenticationService =
            new AuthenticationService(repository);

        string passwordHash =
            authenticationService.HashPassword(
                "CorrectPassword123!");

        repository.AddUser(
            new UserRecord(
                1,
                "user",
                "user@safevault.test",
                passwordHash,
                "User"));

        UserRecord? result =
            await authenticationService.AuthenticateAsync(
                "user",
                "WrongPassword123!");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UnknownUsernameDoesNotAuthenticateUser()
    {
        var repository = new FakeUserRepository();
        var authenticationService =
            new AuthenticationService(repository);

        UserRecord? result =
            await authenticationService.AuthenticateAsync(
                "unknown",
                "Password123!");

        Assert.That(result, Is.Null);
    }
}