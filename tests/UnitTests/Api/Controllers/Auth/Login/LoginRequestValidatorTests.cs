using Api.Controllers.Auth.Login;

namespace UnitTests.Api.Controllers.Auth.Login;

public class LoginRequestValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenBothLoginAndSenhaAreProvided()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "admin", Senha = "senha" };

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
        var request = new LoginRequest { Login = "", Senha = "senha" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Login não pode estar vazio.", result.Errors);
    }

    [Fact]
    public void Validate_HasError_WhenSenhaIsEmpty()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "admin", Senha = "" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Senha não pode estar vazia.", result.Errors);
    }

    [Fact]
    public void Validate_HasBothErrors_WhenBothAreEmpty()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest { Login = "", Senha = "" };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Login não pode estar vazio.", result.Errors);
        Assert.Contains("Senha não pode estar vazia.", result.Errors);
    }
}
