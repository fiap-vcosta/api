using Api.Controllers.Servico.UpdateServico;

namespace UnitTests.Api.Controllers.Servico.UpdateServico;

public class UpdateServicoRequestValidatorTests
{
    private readonly UpdateServicoRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsError_WhenCodigoIsInvalid()
    {
        var request = new UpdateServicoRequest { Codigo = "123-ABC", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Código inválido. Formato esperado: AAA-123", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenPrecoPadraoIsNegative()
    {
        var request = new UpdateServicoRequest { Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = -50.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Preço padrão deve ser maior que zero.", result.Errors);
    }

    [Fact]
    public void Validate_IsValid_WithValidRequest()
    {
        var request = new UpdateServicoRequest { Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
