using Api.Controllers.Veiculo.CreateVeiculo;

namespace UnitTests.Api.Controllers.Veiculo.CreateVeiculo;

public class CreateVeiculoRequestValidatorTests
{
    private readonly CreateVeiculoRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsError_WhenPlacaIsEmpty()
    {
        var request = new CreateVeiculoRequest { Placa = "", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Placa não pode estar vazia.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenPlacaIsInvalid()
    {
        var request = new CreateVeiculoRequest { Placa = "1234", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Placa inválida.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenDonoIdIsInvalid()
    {
        var request = new CreateVeiculoRequest { Placa = "ABC-1D23", DonoId = 0, Modelo = "Gol", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("DonoId deve ser um cliente válido.", result.Errors);
    }

    [Fact]
    public void Validate_IsValid_WithValidRequest()
    {
        var request = new CreateVeiculoRequest { Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
