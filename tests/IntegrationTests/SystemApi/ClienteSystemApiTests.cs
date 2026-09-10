using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.SystemApi;

[Collection(nameof(IntegrationFixture))]
public class ClienteSystemApiTests
{
    private const string ServiceKey = "integration-service-auth-key";
    private readonly HttpClient _client;

    public ClienteSystemApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PorDocumento_ReturnsUnauthorized_WhenServiceKeyMissing()
    {
        // Arrange / Act
        var response = await _client.GetAsync("/api/system/clientes/por-documento/11144477735");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PorDocumento_ReturnsUnauthorized_WhenServiceKeyWrong()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/system/clientes/por-documento/11144477735");
        request.Headers.Add("X-Service-Key", "wrong-key");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PorDocumento_ReturnsBadRequest_WhenDocumentoInvalid()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/system/clientes/por-documento/123");
        request.Headers.Add("X-Service-Key", ServiceKey);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PorDocumento_ReturnsNotFound_WhenClienteDoesNotExist()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/system/clientes/por-documento/52998224725");
        request.Headers.Add("X-Service-Key", ServiceKey);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PorDocumento_ReturnsOk_WhenClienteExists()
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

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/system/clientes/por-documento/{documento}");
        request.Headers.Add("X-Service-Key", ServiceKey);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
