using Api.Controllers.Servico.CreateServico;

namespace UnitTests.Api.Controllers.Servico.CreateServico;

public class CreateServicoRequestValidatorTests
{
    private readonly CreateServicoRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsError_WhenCodigoIsEmpty()
    {
        var request = new CreateServicoRequest { Codigo = "", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Código não pode estar vazio.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenCodigoIsInvalid()
    {
        var request = new CreateServicoRequest { Codigo = "invalid", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Código inválido. Formato esperado: AAA-123", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenNomeIsEmpty()
    {
        var request = new CreateServicoRequest { Codigo = "OLE-001", Nome = "", PrecoPadrao = 150.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Nome não pode estar vazio.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenPrecoPadraoIsZero()
    {
        var request = new CreateServicoRequest { Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 0, Ativo = true };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Preço padrão deve ser maior que zero.", result.Errors);
    }

    [Fact]
    public void Validate_IsValid_WithValidRequest()
    {
        var request = new CreateServicoRequest { Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
