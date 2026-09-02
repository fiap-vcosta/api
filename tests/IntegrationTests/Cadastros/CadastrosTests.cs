using System.Net.Http.Json;
using System.Text.Json;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Cadastros;

[Collection(nameof(IntegrationFixture))]
public class CadastrosTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _client;

    public CadastrosTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Cadastros_CrudHappyPath_AsInDemoCollection()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);

        // CPFs válidos (collection demo) — fora do seed
        const string documento = "92561324354";
        var createCliente = await _client.PostAsJsonAsync("/api/clientes", new
        {
            Nome = "Cliente Integração",
            TipoDocumento = "Cpf",
            Documento = documento
        });
        createCliente.EnsureSuccessStatusCode();
        var cliente = await createCliente.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(cliente);

        var getCliente = await _client.GetAsync($"/api/clientes/{cliente.Id}");
        getCliente.EnsureSuccessStatusCode();

        var updateCliente = await _client.PutAsJsonAsync($"/api/clientes/{cliente.Id}", new
        {
            Nome = "Cliente Integração Atualizado",
            TipoDocumento = "Cpf",
            Documento = "29040653186"
        });
        updateCliente.EnsureSuccessStatusCode();

        var createVeiculo = await _client.PostAsJsonAsync("/api/veiculos", new
        {
            Placa = "ABC-1D23",
            IdCliente = cliente.Id,
            Modelo = "Gol",
            Marca = "Volkswagen"
        });
        createVeiculo.EnsureSuccessStatusCode();
        var veiculo = await createVeiculo.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(veiculo);

        var getVeiculo = await _client.GetAsync($"/api/veiculos/{veiculo.Id}");
        getVeiculo.EnsureSuccessStatusCode();

        var veiculosDoCliente = await _client.GetAsync($"/api/clientes/{cliente.Id}/veiculos");
        veiculosDoCliente.EnsureSuccessStatusCode();

        var getClienteComVeiculos = await _client.GetAsync($"/api/clientes/{cliente.Id}");
        getClienteComVeiculos.EnsureSuccessStatusCode();
        var clienteComVeiculos = await getClienteComVeiculos.Content.ReadFromJsonAsync<ClienteComVeiculosResponse>(JsonOptions);
        Assert.NotNull(clienteComVeiculos);
        Assert.Contains(clienteComVeiculos.Veiculos, v => v.Id == veiculo.Id && v.Placa == "ABC-1D23");

        var createServico = await _client.PostAsJsonAsync("/api/servicos", new
        {
            Codigo = "OLE-999",
            Nome = "Serviço Integração",
            PrecoPadrao = 150m,
            Ativo = true
        });
        createServico.EnsureSuccessStatusCode();
        var servico = await createServico.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(servico);

        var getServico = await _client.GetAsync($"/api/servicos/{servico.Id}");
        getServico.EnsureSuccessStatusCode();

        var createItem = await _client.PostAsJsonAsync("/api/itens-estoque", new
        {
            Codigo = "FLT-999",
            Tipo = "Peca",
            Nome = "Peça Integração",
            UnidadeMedida = "Unidade",
            PrecoVenda = 55.5m,
            Saldo = 10m,
            SaldoReservado = 0m
        });
        createItem.EnsureSuccessStatusCode();
        var item = await createItem.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(item);

        var getItem = await _client.GetAsync($"/api/itens-estoque/{item.Id}");
        getItem.EnsureSuccessStatusCode();

        var entrada = await _client.PostAsJsonAsync($"/api/itens-estoque/{item.Id}/registrar-entrada", new { Quantidade = 50m });
        entrada.EnsureSuccessStatusCode();
    }

    private sealed record IdResponse(int Id);

    private sealed record ClienteComVeiculosResponse(int Id, IReadOnlyList<VeiculoResumoResponse> Veiculos);

    private sealed record VeiculoResumoResponse(int Id, string Placa);
}
