using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SafeVault.Web.Helpers;

public static class ValidationHelpers
{
    private static readonly Regex UsernamePattern =
        new(@"^[a-zA-Z0-9_-]{3,50}$", RegexOptions.Compiled);

    public static bool IsValidUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        return UsernamePattern.IsMatch(username);
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 100)
        {
            return false;
        }

        return MailAddress.TryCreate(email, out var address) &&
               string.Equals(
                   address.Address,
                   email,
                   StringComparison.OrdinalIgnoreCase);
    }

    public static string EncodeForHtml(string? input)
    {
        return WebUtility.HtmlEncode(input ?? string.Empty);
    }
}