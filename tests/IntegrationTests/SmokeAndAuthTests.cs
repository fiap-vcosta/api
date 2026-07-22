using IntegrationTests.Infrastructure;

namespace IntegrationTests;

[Collection(nameof(IntegrationFixture))]
public class SmokeAndAuthTests
{
    private readonly HttpClient _client;

    public SmokeAndAuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Login_ReturnsToken_ForAdmin()
    {
        await AuthHelper.AuthenticateAsAdminAsync(_client);
        Assert.NotNull(_client.DefaultRequestHeaders.Authorization);
    }
}
