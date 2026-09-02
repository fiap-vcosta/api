using System.Net;
using System.Text.Json;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.OrdemServico;

[Collection(nameof(IntegrationFixture))]
public class OrdemServicoApiContratoTests(CustomWebApplicationFactory factory)
    : OrdemServicoIntegrationTestBase(factory)
{
    [Fact]
    public async Task Listagem_OrdersByPriorityAndExcludesFinalStatuses()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var emDiagnosticoId = await CriarOrdemAsync(idVeiculo: 1);
        var aguardandoId = await CriarOrdemAsync(
            idVeiculo: 2,
            servicos:
            [
                new
                {
                    IdServico = 1,
                    ValorCobrado = 100m,
                    ItensNecessarios = Array.Empty<object>()
                }
            ]);
        await FinalizarDiagnosticoAsync(aguardandoId);

        var descartadaId = await CriarOrdemAsync(idVeiculo: 3);
        await DescartarAsync(descartadaId);

        // Act
        var listResponse = await Client.GetAsync("/api/ordens-servico");
        listResponse.EnsureSuccessStatusCode();
        await using var stream = await listResponse.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var ids = document.RootElement.EnumerateArray().Select(e => e.GetProperty("id").GetInt32()).ToList();
        var statuses = document.RootElement.EnumerateArray().Select(e => e.GetProperty("status").GetString()).ToList();

        // Assert
        Assert.Contains(emDiagnosticoId, ids);
        Assert.Contains(aguardandoId, ids);
        Assert.DoesNotContain(descartadaId, ids);
        Assert.DoesNotContain("Descartada", statuses);
        Assert.DoesNotContain("Finalizada", statuses);
        Assert.DoesNotContain("Entregue", statuses);

        var indexAguardando = ids.IndexOf(aguardandoId);
        var indexDiagnostico = ids.IndexOf(emDiagnosticoId);
        Assert.True(indexAguardando < indexDiagnostico);
    }

    [Fact]
    public async Task AprovarPublico_WithInvalidToken_ReturnsNotFound()
    {
        // Arrange / Act
        var response = await Client.PostAsync("/api/public/ordens-servico/aprovar?token=token-inexistente", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminEndpoint_WithoutJwt_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthentication();

        // Act
        var response = await Client.GetAsync("/api/ordens-servico");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
