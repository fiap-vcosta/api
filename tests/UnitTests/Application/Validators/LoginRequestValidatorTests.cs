using Application.Validators;
using Xunit;

namespace UnitTests.Application.Validators;

public class LoginRequestValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenBothLoginAndPasswordAreProvided()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "admin", Password = "password" };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_HasError_WhenLoginIsEmpty()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "", Password = "password" };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Login must not be empty.", result.Errors);
    }

    [Fact]
    public void Validate_HasError_WhenPasswordIsEmpty()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "admin", Password = "" };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Password must not be empty.", result.Errors);
    }

    [Fact]
    public void Validate_HasBothErrors_WhenBothAreEmpty()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "", Password = "" };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Login must not be empty.", result.Errors);
        Assert.Contains("Password must not be empty.", result.Errors);
    }
}
