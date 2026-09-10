using System.Net.Http.Headers;
using System.Security.Claims;
using Api.Auth;
using IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

[Collection(nameof(IntegrationFixture))]
public class ClienteJwtSchemeTests
{
    private readonly CustomWebApplicationFactory _factory;

    public ClienteJwtSchemeTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ClienteScheme_AcceptsTokenSignedWithJwtClientKey()
    {
        // Arrange
        const string cpf = "11144477735";
        var token = ClientJwtHelper.CreateToken(cpf);
        using var scope = _factory.Services.CreateScope();
        var auth = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };
        httpContext.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token).ToString();

        // Act
        var result = await auth.AuthenticateAsync(httpContext, AuthSchemes.Cliente);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(cpf, result.Principal?.FindFirstValue(ClientJwtClaims.Cpf));
    }

    [Fact]
    public async Task ClienteScheme_RejectsStaffToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        await AuthHelper.AuthenticateAsAdminAsync(client);
        var staffToken = client.DefaultRequestHeaders.Authorization!.Parameter!;
        using var scope = _factory.Services.CreateScope();
        var auth = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };
        httpContext.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", staffToken).ToString();

        // Act
        var result = await auth.AuthenticateAsync(httpContext, AuthSchemes.Cliente);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task StaffLogin_StillWorks_WithDualJwtSchemes()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        await AuthHelper.AuthenticateAsAdminAsync(client);

        // Assert
        Assert.NotNull(client.DefaultRequestHeaders.Authorization);
    }
}
