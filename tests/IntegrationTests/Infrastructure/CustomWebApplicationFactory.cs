using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;

    public CustomWebApplicationFactory()
    {
        EnsureDockerHost();
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("techchallenge_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync() => await _postgres.StartAsync();

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                ["Jwt:Key"] = "a-really-long-dummy-token-key-that-should-be-kept-secret",
                ["Jwt:Issuer"] = "dummy-issuer",
                ["Jwt:Audience"] = "dummy-audience"
            });
        });
    }

    private static void EnsureDockerHost()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOCKER_HOST")))
        {
            return;
        }

        if (File.Exists("/var/run/docker.sock"))
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", "unix:///var/run/docker.sock");
        }
    }
}

[CollectionDefinition(nameof(IntegrationFixture))]
public class IntegrationFixture : ICollectionFixture<CustomWebApplicationFactory>;
