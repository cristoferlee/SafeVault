using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthorization
{
    [Test]
    public async Task AdminRoleCanAccessAdminPolicy()
    {
        IAuthorizationService authorizationService =
            CreateAuthorizationService();

        ClaimsPrincipal admin =
            CreateUserWithRole("Admin");

        AuthorizationResult result =
            await authorizationService.AuthorizeAsync(
                admin,
                resource: null,
                policyName: "AdminOnly");

        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public async Task UserRoleCannotAccessAdminPolicy()
    {
        IAuthorizationService authorizationService =
            CreateAuthorizationService();

        ClaimsPrincipal user =
            CreateUserWithRole("User");

        AuthorizationResult result =
            await authorizationService.AuthorizeAsync(
                user,
                resource: null,
                policyName: "AdminOnly");

        Assert.That(result.Succeeded, Is.False);
    }

    private static IAuthorizationService
        CreateAuthorizationService()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "AdminOnly",
                policy => policy.RequireRole("Admin"));
        });

        ServiceProvider provider =
            services.BuildServiceProvider();

        return provider.GetRequiredService<
            IAuthorizationService>();
    }

    private static ClaimsPrincipal CreateUserWithRole(
        string role)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(ClaimTypes.Role, role)
        ];

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "Test");

        return new ClaimsPrincipal(identity);
    }
}