using NUnit.Framework;
using SafeVault.Web.Helpers;

namespace SafeVault.Tests;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        const string maliciousInput = "' OR 1=1 --";

        bool isValid =
            ValidationHelpers.IsValidUsername(maliciousInput);

        Assert.That(isValid, Is.False);
    }

    [Test]
    public void TestForXSS()
    {
        const string maliciousInput =
            "<script>alert('XSS');</script>";

        string encodedOutput =
            ValidationHelpers.EncodeForHtml(maliciousInput);

        Assert.Multiple(() =>
        {
            Assert.That(
                encodedOutput,
                Does.Not.Contain("<script>"));

            Assert.That(
                encodedOutput,
                Does.Contain("&lt;script&gt;"));
        });
    }

    [TestCase("lee_2026")]
    [TestCase("Safe-Vault")]
    [TestCase("User123")]
    public void ValidUsernamesAreAccepted(string username)
    {
        bool isValid =
            ValidationHelpers.IsValidUsername(username);

        Assert.That(isValid, Is.True);
    }

    [TestCase("lee@example.com")]
    [TestCase("user.name@safevault.test")]
    public void ValidEmailsAreAccepted(string email)
    {
        bool isValid =
            ValidationHelpers.IsValidEmail(email);

        Assert.That(isValid, Is.True);
    }
}