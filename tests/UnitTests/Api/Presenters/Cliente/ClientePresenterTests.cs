using Api.Presenters.Cliente;
using Application.UseCases.Administrativo.Cliente.Responses;
using Domain.Administrativo.Entities;

namespace UnitTests.Api.Presenters.Cliente;

public class ClientePresenterTests
{
    private readonly ClientePresenter _presenter = new();

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
