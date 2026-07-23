using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SafeVault.Tests;

[TestFixture]
public class TestSecurityEndpoints
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>();

        _client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task SubmitRejectsSQLInjection()
    {
        var formData = new Dictionary<string, string>
        {
            ["username"] = "' OR 1=1 --",
            ["email"] = "user@safevault.test"
        };

        using var content =
            new FormUrlEncodedContent(formData);

        HttpResponseMessage response =
            await _client.PostAsync("/submit", content);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task SubmitRejectsXSSPayload()
    {
        var formData = new Dictionary<string, string>
        {
            ["username"] = "<script>alert('XSS')</script>",
            ["email"] = "user@safevault.test"
        };

        using var content =
            new FormUrlEncodedContent(formData);

        HttpResponseMessage response =
            await _client.PostAsync("/submit", content);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RegisterRejectsXSSPayload()
    {
        var request = new
        {
            Username = "<script>alert('XSS')</script>",
            Email = "user@safevault.test",
            Password = "SecurePassword123!"
        };

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/register",
                request);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task LoginRejectsSQLInjection()
    {
        var request = new
        {
            Username = "' OR 1=1 --",
            Password = "SecurePassword123!"
        };

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/login",
                request);

        Assert.That(
            response.StatusCode,
            Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}