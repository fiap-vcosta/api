using Infrastructure.Services;

namespace UnitTests.Infrastructure.Services;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsConsistentHash_ForSamePassword()
    {
        // Arrange
        var password = "MySecurePassword123";

        // Act
        var hash1 = PasswordHasher.HashPassword(password);
        var hash2 = PasswordHasher.HashPassword(password);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ReturnsDifferentHashes_ForDifferentPasswords()
    {
        // Arrange
        var firstPassword = "password1";
        var secondPassword = "password2";

        // Act
        var hash1 = PasswordHasher.HashPassword(firstPassword);
        var hash2 = PasswordHasher.HashPassword(secondPassword);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ReturnsNonEmptyString()
    {
        // Arrange
        var password = "password";

        // Act
        var hash = PasswordHasher.HashPassword(password);

        // Assert
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_ReturnsHexString()
    {
        // Arrange
        var password = "password";

        // Act
        var hash = PasswordHasher.HashPassword(password);

        // Assert
        Assert.Matches("^[a-f0-9]+$", hash);
    }

    [Fact]
    public void HashPassword_ReturnsSHA256Length_64Characters()
    {
        // Arrange
        var password = "password";

        // Act
        var hash = PasswordHasher.HashPassword(password);

        // Assert
        Assert.Equal(64, hash.Length);
    }
}
