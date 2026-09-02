using Api.Presenters.Cliente;
using Api.Presenters.Veiculo;
using Application.UseCases.Administrativo.Cliente.Responses;
using Application.UseCases.Administrativo.Veiculo.Responses;
using Domain.Administrativo.Entities;

namespace UnitTests.Api.Presenters.Cliente;

public class ClientePresenterTests
{
    private readonly ClientePresenter _presenter = new(new VeiculoPresenter());

    [Fact]
    public void Present_MapsResponseToViewModel()
    {
        // Arrange
        var response = new ClienteResponse
        {
            Id = 7,
            Nome = "Maria",
            TipoDocumento = TipoDocumento.Cpf,
            Documento = "11144477735"
        };

        // Act
        var viewModel = _presenter.Present(response);

        // Assert
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Nome, viewModel.Nome);
        Assert.Equal(response.TipoDocumento, viewModel.TipoDocumento);
        Assert.Equal(response.Documento, viewModel.Documento);
        Assert.Empty(viewModel.Veiculos);
    }

    [Fact]
    public void Present_MapsVeiculos_WhenPresentInResponse()
    {
        // Arrange
        var response = new ClienteResponse
        {
            Id = 7,
            Nome = "Maria",
            TipoDocumento = TipoDocumento.Cpf,
            Documento = "11144477735",
            Veiculos =
            [
                new VeiculoResponse { Id = 1, Placa = "ABC-1D23", IdCliente = 7, Modelo = "Gol", Marca = "Volkswagen" }
            ]
        };

        // Act
        var viewModel = _presenter.Present(response);

        // Assert
        Assert.NotNull(viewModel.Veiculos);
        Assert.Single(viewModel.Veiculos);
        Assert.Equal(1, viewModel.Veiculos[0].Id);
        Assert.Equal("ABC-1D23", viewModel.Veiculos[0].Placa);
    }

    [Fact]
    public void Present_MapsCollectionToViewModels()
    {
        // Arrange
        var responses = new List<ClienteResponse>
        {
            new() { Id = 1, Nome = "A", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" },
            new() { Id = 2, Nome = "B", TipoDocumento = TipoDocumento.Cnpj, Documento = "12345678901234" }
        };

        // Act
        var viewModels = _presenter.Present(responses).ToList();

        // Assert
        Assert.Equal(2, viewModels.Count);
        Assert.Equal(responses[0].Id, viewModels[0].Id);
        Assert.Equal(responses[1].Nome, viewModels[1].Nome);
    }
}
