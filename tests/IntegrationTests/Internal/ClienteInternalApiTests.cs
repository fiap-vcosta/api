using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Internal;

[Collection(nameof(IntegrationFixture))]
public class ClienteInternalApiTests
{
    private const string ServiceKey = "integration-service-auth-key";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _client;

    public ClienteInternalApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PorDocumento_ReturnsUnauthorized_WhenServiceKeyMissing()
    {
        // Arrange / Act
        var response = await _client.GetAsync("/api/internal/clientes/por-documento/11144477735");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PorDocumento_ReturnsUnauthorized_WhenServiceKeyWrong()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/internal/clientes/por-documento/11144477735");
        request.Headers.Add("X-Service-Key", "wrong-key");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PorDocumento_ReturnsExisteFalse_WhenClienteDoesNotExist()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/internal/clientes/por-documento/99999999999");
        request.Headers.Add("X-Service-Key", ServiceKey);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PorDocumentoResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.False(body.Existe);
    }

    [Fact]
    public async Task PorDocumento_ReturnsExisteTrue_WhenClienteExists()
    {
        // Arrange
        await AuthHelper.AuthenticateAsAdminAsync(_client);
        const string documento = "39053344705";
        var create = await _client.PostAsJsonAsync("/api/clientes", new
        {
            Nome = "Cliente RF23",
            TipoDocumento = "Cpf",
            Documento = documento
        });
        create.EnsureSuccessStatusCode();
        _client.DefaultRequestHeaders.Authorization = null;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/internal/clientes/por-documento/{documento}");
        request.Headers.Add("X-Service-Key", ServiceKey);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<PorDocumentoResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body.Existe);
        Assert.Equal(documento, body.Documento);
        Assert.Equal("Cpf", body.TipoDocumento);
    }

    private sealed record PorDocumentoResponse(bool Existe, string? Documento, string? TipoDocumento);
}
