using Infrastructure.Services;
using Xunit;

namespace UnitTests.Infrastructure.Services;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsConsistentHash_ForSamePassword()
    {
        var password = "MySecurePassword123";

        var hash1 = PasswordHasher.HashPassword(password);
        var hash2 = PasswordHasher.HashPassword(password);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ReturnsDifferentHashes_ForDifferentPasswords()
    {
        var hash1 = PasswordHasher.HashPassword("password1");
        var hash2 = PasswordHasher.HashPassword("password2");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ReturnsNonEmptyString()
    {
        var hash = PasswordHasher.HashPassword("password");

        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_ReturnsHexString()
    {
        var hash = PasswordHasher.HashPassword("password");

        Assert.Matches("^[a-f0-9]+$", hash);
    }

    [Fact]
    public void HashPassword_ReturnsSHA256Length_64Characters()
    {
        var hash = PasswordHasher.HashPassword("password");

        Assert.Equal(64, hash.Length);
    }
}
