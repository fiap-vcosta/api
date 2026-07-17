using Api.Presenters.ItemEstoque;
using Application.UseCases.Estoque.ItemEstoque.Responses;
using Domain.Estoque.Entities;

namespace UnitTests.Api.Presenters.ItemEstoque;

public class ItemEstoquePresenterTests
{
    private readonly ItemEstoquePresenter _presenter = new();

    [Fact]
    public void Present_MapsResponseToViewModel()
    {
        // Arrange
        var response = new ItemEstoqueResponse
        {
            Id = 3,
            Codigo = "ITM-003",
            Tipo = ItemTipo.Peca,
            Nome = "Filtro de óleo",
            UnidadeMedida = UnidadeMedida.Unidade,
            PrecoVenda = 45.90m,
            Saldo = 12m,
            SaldoReservado = 2m
        };

        // Act
        var viewModel = _presenter.Present(response);

        // Assert
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Codigo, viewModel.Codigo);
        Assert.Equal(response.Tipo, viewModel.Tipo);
        Assert.Equal(response.Nome, viewModel.Nome);
        Assert.Equal(response.UnidadeMedida, viewModel.UnidadeMedida);
        Assert.Equal(response.PrecoVenda, viewModel.PrecoVenda);
        Assert.Equal(response.Saldo, viewModel.Saldo);
        Assert.Equal(response.SaldoReservado, viewModel.SaldoReservado);
    }

    [Fact]
    public void Present_MapsCollectionToViewModels()
    {
        // Arrange
        var responses = new List<ItemEstoqueResponse>
        {
            new()
            {
                Id = 1,
                Codigo = "ITM-001",
                Tipo = ItemTipo.Peca,
                Nome = "Filtro",
                UnidadeMedida = UnidadeMedida.Unidade,
                PrecoVenda = 10m,
                Saldo = 5m,
                SaldoReservado = 0m
            },
            new()
            {
                Id = 2,
                Codigo = "ITM-002",
                Tipo = ItemTipo.Insumo,
                Nome = "Óleo",
                UnidadeMedida = UnidadeMedida.Litro,
                PrecoVenda = 20m,
                Saldo = 8m,
                SaldoReservado = 1m
            }
        };

        // Act
        var viewModels = _presenter.Present(responses).ToList();

        // Assert
        Assert.Equal(2, viewModels.Count);
        Assert.Equal(responses[0].Id, viewModels[0].Id);
        Assert.Equal(responses[1].Codigo, viewModels[1].Codigo);
    }
}
