using Api.Controllers.Cliente;
using Domain.Entities;

namespace UnitTests.Api.Validators;

public class UpdateClienteRequestValidatorTests
{
    private readonly UpdateClienteRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsError_WhenCpfIsInvalid()
    {
        // Arrange
        var request = new UpdateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = (int)TipoDocumento.Cpf,
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
        var request = new UpdateClienteRequest
        {
            Nome = "Cliente Teste",
            TipoDocumento = (int)TipoDocumento.Cnpj,
            Documento = "00000000000"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("CNPJ inválido.", result.Errors);
    }

    [Fact]
    public void Validate_IsValid_WithValidCPFAndValidId()
    {
        // Arrange
        var request = new UpdateClienteRequest
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

    [Fact]
    public void Validate_IsValid_WhenDocumentoIsNull()
    {
        // Arrange
        var request = new UpdateClienteRequest
        {
            Nome = "Cliente Teste Atualizado"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }
}
