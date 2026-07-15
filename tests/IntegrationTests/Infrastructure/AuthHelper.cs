using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTests.Infrastructure;

public static class AuthHelper
{
    public static async Task AuthenticateAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/Auth/login", new
        {
            login = "admin",
            password = "admin"
        });

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        var token = document.RootElement.GetProperty("token").GetString()
            ?? throw new InvalidOperationException("Token não retornado no login.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
