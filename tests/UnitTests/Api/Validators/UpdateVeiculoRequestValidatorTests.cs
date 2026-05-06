using Api.Controllers.Veiculo;

namespace UnitTests.Api.Validators;

public class UpdateVeiculoRequestValidatorTests
{
    private readonly UpdateVeiculoRequestValidator _validator = new();

    [Fact]
    public void Validate_ReturnsError_WhenMarcaIsEmpty()
    {
        var request = new UpdateVeiculoRequest { Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Marca não pode estar vazia.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenModeloIsEmpty()
    {
        var request = new UpdateVeiculoRequest { Placa = "ABC-1D23", DonoId = 1, Modelo = "", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Modelo não pode estar vazio.", result.Errors);
    }

    [Fact]
    public void Validate_ReturnsError_WhenDonoIdIsInvalid()
    {
        var request = new UpdateVeiculoRequest { Placa = "ABC-1D23", DonoId = 0, Modelo = "Gol", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("DonoId deve ser um cliente válido.", result.Errors);
    }

    [Fact]
    public void Validate_IsValid_WithValidRequest()
    {
        var request = new UpdateVeiculoRequest { Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
