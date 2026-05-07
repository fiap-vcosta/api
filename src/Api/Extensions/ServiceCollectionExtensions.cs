using Application.Usuario.Commands;
using Application.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Contracts;
using Api.Controllers.Auth;
using Api.Controllers.Cliente;
using Api.Controllers.ItemEstoque;
using Api.Controllers.Servico;
using Api.Controllers.Veiculo;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddCoreServices();
        services.AddUsuarioServices();
        services.AddClienteServices();
        services.AddVeiculoServices();
        services.AddServicoServices();
        services.AddItemEstoqueServices();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
        services.AddControllers();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IValidator<LoginRequest>, LoginRequestValidator>();
        return services;
    }

    private static IServiceCollection AddUsuarioServices(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        return services;
    }

    private static IServiceCollection AddClienteServices(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddSingleton<IValidator<CreateClienteRequest>, CreateClienteRequestValidator>();
        services.AddSingleton<IValidator<UpdateClienteRequest>, UpdateClienteRequestValidator>();
        return services;
    }

    private static IServiceCollection AddVeiculoServices(this IServiceCollection services)
    {
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddSingleton<IValidator<CreateVeiculoRequest>, CreateVeiculoRequestValidator>();
        services.AddSingleton<IValidator<UpdateVeiculoRequest>, UpdateVeiculoRequestValidator>();
        return services;
    }

    private static IServiceCollection AddServicoServices(this IServiceCollection services)
    {
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddSingleton<IValidator<CreateServicoRequest>, CreateServicoRequestValidator>();
        services.AddSingleton<IValidator<UpdateServicoRequest>, UpdateServicoRequestValidator>();
        return services;
    }

    private static IServiceCollection AddItemEstoqueServices(this IServiceCollection services)
    {
        services.AddScoped<IItemEstoqueRepository, ItemEstoqueRepository>();
        services.AddSingleton<IValidator<CreateItemEstoqueRequest>, CreateItemEstoqueRequestValidator>();
        services.AddSingleton<IValidator<UpdateItemEstoqueRequest>, UpdateItemEstoqueRequestValidator>();
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? "default-key";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "default-issuer";
        var jwtAudience = configuration["Jwt:Audience"] ?? "default-audience";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience
                };
            });

        return services;
    }
}
