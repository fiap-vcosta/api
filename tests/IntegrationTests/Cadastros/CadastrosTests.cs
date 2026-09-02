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
        var createCliente = await _client.PostAsJsonAsync("/api/Cliente", new
        {
            Nome = "Cliente Integração",
            TipoDocumento = "Cpf",
            Documento = documento
        });
        createCliente.EnsureSuccessStatusCode();
        var cliente = await createCliente.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(cliente);

        var getCliente = await _client.GetAsync($"/api/Cliente/{cliente.Id}");
        getCliente.EnsureSuccessStatusCode();

        var updateCliente = await _client.PutAsJsonAsync($"/api/Cliente/{cliente.Id}", new
        {
            Nome = "Cliente Integração Atualizado",
            TipoDocumento = "Cpf",
            Documento = "29040653186"
        });
        updateCliente.EnsureSuccessStatusCode();

        var createVeiculo = await _client.PostAsJsonAsync("/api/Veiculo", new
        {
            Placa = "ABC-1D23",
            IdCliente = cliente.Id,
            Modelo = "Gol",
            Marca = "Volkswagen"
        });
        createVeiculo.EnsureSuccessStatusCode();
        var veiculo = await createVeiculo.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(veiculo);

        var getVeiculo = await _client.GetAsync($"/api/Veiculo/{veiculo.Id}");
        getVeiculo.EnsureSuccessStatusCode();

        var porDono = await _client.GetAsync($"/api/Veiculo/por-dono/{cliente.Id}");
        porDono.EnsureSuccessStatusCode();

        var createServico = await _client.PostAsJsonAsync("/api/Servico", new
        {
            Codigo = "OLE-999",
            Nome = "Serviço Integração",
            PrecoPadrao = 150m,
            Ativo = true
        });
        createServico.EnsureSuccessStatusCode();
        var servico = await createServico.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(servico);

        var getServico = await _client.GetAsync($"/api/Servico/{servico.Id}");
        getServico.EnsureSuccessStatusCode();

        var createItem = await _client.PostAsJsonAsync("/api/ItemEstoque", new
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

        var getItem = await _client.GetAsync($"/api/ItemEstoque/{item.Id}");
        getItem.EnsureSuccessStatusCode();

        var entrada = await _client.PostAsJsonAsync($"/api/ItemEstoque/{item.Id}/registrar-entrada", new { Quantidade = 50m });
        entrada.EnsureSuccessStatusCode();
    }

    private sealed record IdResponse(int Id);
}
