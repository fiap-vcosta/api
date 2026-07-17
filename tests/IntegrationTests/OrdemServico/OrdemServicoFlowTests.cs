using System.Net.Http.Json;
using System.Text.Json;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.OrdemServico;

[Collection(nameof(IntegrationFixture))]
public class OrdemServicoFlowTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _client;

    public OrdemServicoFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CicloFeliz_CreateDiagnosticoAprovarExecutarPagar()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);

        var ordemId = await CriarOrdemAsync(idVeiculo: 1);
        await AssertStatusAsync(ordemId, "EmDiagnostico");

        await AdicionarServicoAsync(ordemId, idServico: 1, valorCobrado: 150m, idItemEstoque: 1, quantidade: 2m);
        await FinalizarDiagnosticoAsync(ordemId);
        await AssertStatusAsync(ordemId, "AguardandoAprovacao");

        await AprovarAsync(ordemId);
        await AssertStatusAsync(ordemId, "LiberadaParaExecucao");

        var servicoOsId = await GetPrimeiroServicoIdAsync(ordemId);
        await ConfirmarExecucaoAsync(ordemId, servicoOsId);
        await AssertStatusAsync(ordemId, "Finalizada");

        var pagamento = await _client.PostAsync($"/api/OrdemServico/{ordemId}/confirmar-pagamento", null);
        pagamento.EnsureSuccessStatusCode();
        await AssertStatusAsync(ordemId, "Entregue");

        var tempoMedio = await _client.GetAsync("/api/OrdemServico/tempo-medio-execucao");
        tempoMedio.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Rejeitar_ReturnsToDiagnostico()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);

        var ordemId = await CriarOrdemAsync(idVeiculo: 2);
        await AdicionarServicoAsync(ordemId, idServico: 2, valorCobrado: 50m, idItemEstoque: null, quantidade: 0);
        await FinalizarDiagnosticoAsync(ordemId);

        var rejeitar = await _client.PostAsync($"/api/OrdemServico/{ordemId}/rejeitar", null);
        rejeitar.EnsureSuccessStatusCode();
        await AssertStatusAsync(ordemId, "EmDiagnostico");
    }

    [Fact]
    public async Task AprovarParcialmente_ReturnsToDiagnostico()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);

        var ordemId = await CriarOrdemAsync(idVeiculo: 3);
        await AdicionarServicoAsync(ordemId, idServico: 1, valorCobrado: 100m, idItemEstoque: null, quantidade: 0);
        await AdicionarServicoAsync(ordemId, idServico: 2, valorCobrado: 50m, idItemEstoque: null, quantidade: 0);
        await FinalizarDiagnosticoAsync(ordemId);

        var primeiroServicoId = await GetPrimeiroServicoIdAsync(ordemId);
        var response = await _client.PostAsJsonAsync($"/api/OrdemServico/{ordemId}/aprovar-parcialmente", new
        {
            IdsServicosAprovados = new[] { primeiroServicoId }
        });
        response.EnsureSuccessStatusCode();
        await AssertStatusAsync(ordemId, "EmDiagnostico");
    }

    [Fact]
    public async Task Descartar_WhenEmDiagnostico()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);

        var ordemId = await CriarOrdemAsync(idVeiculo: 4);
        var response = await _client.PostAsync($"/api/OrdemServico/{ordemId}/descartar", null);
        response.EnsureSuccessStatusCode();
        await AssertStatusAsync(ordemId, "Descartada");
    }

    [Fact]
    public async Task Aprovar_WhenStockInsufficient_GoesToAguardandoPeca()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);

        // Seed item 2 (INS-002) tem saldo 0
        var ordemId = await CriarOrdemAsync(idVeiculo: 5);
        await AdicionarServicoAsync(ordemId, idServico: 1, valorCobrado: 150m, idItemEstoque: 2, quantidade: 1m);
        await FinalizarDiagnosticoAsync(ordemId);
        await AprovarAsync(ordemId);
        await AssertStatusAsync(ordemId, "AguardandoPeca");
    }

    private async Task<int> CriarOrdemAsync(int idVeiculo)
    {
        var response = await _client.PostAsJsonAsync("/api/OrdemServico", new { IdVeiculo = idVeiculo });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>(JsonOptions);
        Assert.NotNull(body);
        return body.Id;
    }

    private async Task AdicionarServicoAsync(int ordemId, int idServico, decimal valorCobrado, int? idItemEstoque, decimal quantidade)
    {
        object request = idItemEstoque is null
            ? new { IdServico = idServico, ValorCobrado = valorCobrado, ItensNecessarios = Array.Empty<object>() }
            : new
            {
                IdServico = idServico,
                ValorCobrado = valorCobrado,
                ItensNecessarios = new[] { new { IdItemEstoque = idItemEstoque.Value, Quantidade = quantidade } }
            };

        var response = await _client.PostAsJsonAsync($"/api/OrdemServico/{ordemId}/adicionar-servico", request);
        response.EnsureSuccessStatusCode();
    }

    private async Task FinalizarDiagnosticoAsync(int ordemId)
    {
        var response = await _client.PostAsync($"/api/OrdemServico/{ordemId}/finalizar-diagnostico", null);
        response.EnsureSuccessStatusCode();
    }

    private async Task AprovarAsync(int ordemId)
    {
        var response = await _client.PostAsync($"/api/OrdemServico/{ordemId}/aprovar", null);
        response.EnsureSuccessStatusCode();
    }

    private async Task ConfirmarExecucaoAsync(int ordemId, int idServico)
    {
        var response = await _client.PostAsJsonAsync($"/api/OrdemServico/{ordemId}/confirmar-execucao", new
        {
            ServicosExecutados = new[]
            {
                new
                {
                    IdServico = idServico,
                    IniciadoEm = DateTime.UtcNow.AddHours(-1),
                    FinalizadoEm = DateTime.UtcNow
                }
            }
        });
        response.EnsureSuccessStatusCode();
    }

    private async Task AssertStatusAsync(int ordemId, string expectedStatus)
    {
        var response = await _client.GetAsync($"/api/OrdemServico/{ordemId}");
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var status = document.RootElement.GetProperty("status").GetString();
        Assert.Equal(expectedStatus, status);
    }

    private async Task<int> GetPrimeiroServicoIdAsync(int ordemId)
    {
        var response = await _client.GetAsync($"/api/OrdemServico/{ordemId}");
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var servicos = document.RootElement.GetProperty("servicos");
        Assert.True(servicos.GetArrayLength() > 0);
        return servicos[0].GetProperty("id").GetInt32();
    }

    private sealed record IdResponse(int Id);
}
