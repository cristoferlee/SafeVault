using SafeVault.Web.Helpers;
using SafeVault.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<SafeVault.Web.Data.UserRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

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

app.Run();

public partial class Program;