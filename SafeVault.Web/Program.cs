using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SafeVault.Web.Data;
using SafeVault.Web.Helpers;
using SafeVault.Web.Models;

using AuthenticationService =
    SafeVault.Web.Services.AuthenticationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/submit", async Task<IResult> (HttpRequest request) =>
{
    if (!request.HasFormContentType)
    {
        return Results.BadRequest("Invalid form content.");
    }

    var form = await request.ReadFormAsync();

    var input = new UserInput
    {
        Username = form["username"].ToString().Trim(),
        Email = form["email"].ToString().Trim()
    };

    if (!ValidationHelpers.IsValidUsername(input.Username))
    {
        return Results.BadRequest(
            "Username must contain 3 to 50 letters, numbers, underscores, or hyphens.");
    }

    if (!ValidationHelpers.IsValidEmail(input.Email))
    {
        return Results.BadRequest("The email address is invalid.");
    }

    string safeUsername =
        ValidationHelpers.EncodeForHtml(input.Username);

    string safeEmail =
        ValidationHelpers.EncodeForHtml(input.Email);

    string responseHtml = $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <title>SafeVault Result</title>
        </head>
        <body>
            <main>
                <h1>Submission accepted</h1>
                <p>Username: {safeUsername}</p>
                <p>Email: {safeEmail}</p>
                <a href="/">Return to form</a>
            </main>
        </body>
        </html>
        """;

    return Results.Content(
        responseHtml,
        contentType: "text/html; charset=utf-8");
});

app.MapPost(
    "/register",
    async (
        RegisterRequest request,
        AuthenticationService authenticationService,
        IUserRepository userRepository,
        CancellationToken cancellationToken) =>
    {
        if (!ValidationHelpers.IsValidUsername(request.Username) ||
            !ValidationHelpers.IsValidEmail(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 12)
        {
            return Results.BadRequest("Invalid registration data.");
        }

        string passwordHash =
            authenticationService.HashPassword(request.Password);

        await userRepository.CreateAsync(
            request.Username,
            request.Email,
            passwordHash,
            role: "User",
            cancellationToken);

        return Results.Created(
            "/login",
            new { message = "User registered successfully." });
    });

app.MapPost(
    "/login",
    async (
        LoginRequest request,
        AuthenticationService authenticationService,
        HttpContext httpContext,
        CancellationToken cancellationToken) =>
    {
        UserRecord? user =
            await authenticationService.AuthenticateAsync(
                request.Username,
                request.Password,
                cancellationToken);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        Claim[] claims =
        [
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.Username),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        ];

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return Results.Ok(
            new
            {
                message = "Login successful.",
                role = user.Role
            });
    });

app.MapPost(
    "/logout",
    async (HttpContext httpContext) =>
    {
        await httpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Results.Ok(
            new { message = "Logout successful." });
    })
    .RequireAuthorization();

app.MapGet(
    "/admin",
    () => Results.Ok(
        new { message = "Welcome to the Admin Dashboard." }))
    .RequireAuthorization("AdminOnly");

app.MapGet(
    "/access-denied",
    () => Results.StatusCode(StatusCodes.Status403Forbidden));


app.Run();

public partial class Program;