using Api.Controllers.Cliente.CreateCliente;
using Domain.Administrativo.Entities;

namespace UnitTests.Api.Controllers.Cliente.CreateCliente;

public class CreateClienteRequestValidatorTests
{
    private readonly CreateClienteRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsError_WhenNomeIsEmpty()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nome = "",
            TipoDocumento = 0,
            Documento = "11144477735"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Nome não pode estar vazio.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenDocumentoIsEmpty()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = 0,
            Documento = ""
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Documento não pode estar vazio.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenTipoDocumentoIsInvalid()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = (TipoDocumento)999,
            Documento = "11144477735"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("TipoDocumento é inválido.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenCpfIsInvalid()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = 0, // CPF
            Documento = "00000000000"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("CPF inválido.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenCnpjIsInvalid()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = TipoDocumento.Cnpj,
            Documento = "00000000000000"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("CNPJ inválido.", result.Errors);
    }

    [Fact]
    public void Validate_IsValid_WithValidCPF()
    {
        // Arrange - Using a valid test CPF
        var request = new CreateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = 0, // CPF
            Documento = "11144477735"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
