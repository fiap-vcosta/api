using Api.Presenters.Veiculo;
using Application.UseCases.Administrativo.Veiculo.Responses;

namespace UnitTests.Api.Presenters.Veiculo;

public class VeiculoPresenterTests
{
    private readonly VeiculoPresenter _presenter = new();

    [Fact]
    public void Present_MapsResponseToViewModel()
    {
        // Arrange
        var response = new VeiculoResponse
        {
            Id = 7,
            IdDono = 3,
            Placa = "ABC-1D23",
            Modelo = "Gol",
            Marca = "Volkswagen"
        };

        // Act
        var viewModel = _presenter.Present(response);

        // Assert
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.IdDono, viewModel.IdDono);
        Assert.Equal(response.Placa, viewModel.Placa);
        Assert.Equal(response.Modelo, viewModel.Modelo);
        Assert.Equal(response.Marca, viewModel.Marca);
    }

    [Fact]
    public void Present_MapsCollectionToViewModels()
    {
        // Arrange
        var responses = new List<VeiculoResponse>
        {
            new() { Id = 1, IdDono = 1, Placa = "ABC-1D23", Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, IdDono = 2, Placa = "DEF-2G34", Modelo = "Polo", Marca = "Volkswagen" }
        };

        // Act
        var viewModels = _presenter.Present(responses).ToList();

        // Assert
        Assert.Equal(2, viewModels.Count);
        Assert.Equal(responses[0].Id, viewModels[0].Id);
        Assert.Equal(responses[1].Placa, viewModels[1].Placa);
    }
}
