using Api.Validators;

namespace UnitTests.Api.Validators;

public class LoginRequestValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenBothLoginAndPasswordAreProvided()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "admin", Password = "password" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_HasError_WhenLoginIsEmpty()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "", Password = "password" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Login must not be empty.", result.Errors);
    }

    [Fact]
    public void Validate_HasError_WhenPasswordIsEmpty()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "admin", Password = "" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Password must not be empty.", result.Errors);
    }

    [Fact]
    public void Validate_HasBothErrors_WhenBothAreEmpty()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "", Password = "" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Login must not be empty.", result.Errors);
        Assert.Contains("Password must not be empty.", result.Errors);
    }
}
