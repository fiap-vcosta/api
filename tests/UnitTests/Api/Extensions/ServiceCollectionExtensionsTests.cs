using Api.Contracts.Validation;
using Api.Controllers.OrdemServico.CriarOrdemServico;
using Api.Extensions;
using Application.Abstractions.Services;
using Domain.Administrativo.Repositories;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Api.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApiServices_RegistersCoreDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=postgres;Password=postgres",
                ["Jwt:Key"] = "super-secret-key-1234567890-ABCDEFGH",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddApiServices(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var scoped = scope.ServiceProvider;

        // Assert
        Assert.NotNull(provider.GetService<IJwtService>());
        Assert.NotNull(provider.GetService<INotificacaoService>());
        Assert.NotNull(provider.GetService<ISMTPService>());
        Assert.NotNull(provider.GetService<IValidator<CriarOrdemServicoRequest>>());
        Assert.NotNull(scoped.GetRequiredService<IClienteRepository>());
        Assert.NotNull(scoped.GetRequiredService<IVeiculoRepository>());
        Assert.NotNull(scoped.GetRequiredService<IServicoRepository>());
        Assert.NotNull(scoped.GetRequiredService<IItemEstoqueRepository>());
        Assert.NotNull(scoped.GetRequiredService<IOrdemServicoRepository>());
        Assert.NotNull(scoped.GetRequiredService<IItemServicoRepository>());
        Assert.NotNull(scoped.GetRequiredService<IUsuarioRepository>());
    }
}
