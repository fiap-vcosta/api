using Api.Presenters.Servico;
using Application.UseCases.Administrativo.Servico.Responses;

namespace UnitTests.Api.Presenters.Servico;

public class ServicoPresenterTests
{
    private readonly ServicoPresenter _presenter = new();

    [Fact]
    public void Present_MapsResponseToViewModel()
    {
        // Arrange
        var response = new ServicoResponse
        {
            Id = 7,
            Codigo = "OLE-001",
            Nome = "Óleo",
            PrecoPadrao = 150.00m,
            Ativo = true
        };

        // Act
        var viewModel = _presenter.Present(response);

        // Assert
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Codigo, viewModel.Codigo);
        Assert.Equal(response.Nome, viewModel.Nome);
        Assert.Equal(response.PrecoPadrao, viewModel.PrecoPadrao);
        Assert.Equal(response.Ativo, viewModel.Ativo);
    }

    [Fact]
    public void Present_MapsCollectionToViewModels()
    {
        // Arrange
        var responses = new List<ServicoResponse>
        {
            new() { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true },
            new() { Id = 2, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true }
        };

        // Act
        var viewModels = _presenter.Present(responses).ToList();

        // Assert
        Assert.Equal(2, viewModels.Count);
        Assert.Equal(responses[0].Id, viewModels[0].Id);
        Assert.Equal(responses[1].Nome, viewModels[1].Nome);
    }
}
