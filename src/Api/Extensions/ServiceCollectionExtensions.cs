using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Contracts.Validation;
using Api.Controllers.Auth.Login;
using Api.Controllers.Cliente.CreateCliente;
using Api.Controllers.Cliente.UpdateCliente;
using Api.Controllers.ItemEstoque.CreateItemEstoque;
using Api.Controllers.ItemEstoque.UpdateItemEstoque;
using Api.Controllers.Servico.CreateServico;
using Api.Controllers.Servico.UpdateServico;
using Api.Controllers.Veiculo.CreateVeiculo;
using Api.Controllers.Veiculo.UpdateVeiculo;
using Application.Abstractions.Services;
using Application.Administrativo.Usuario.Commands.Login;
using Domain.Administrativo.Repositories;
using Domain.Estoque.Repositories;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
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
    }

    private static void AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<INotificacaoService, NotificacaoService>();
        services.AddSingleton<ISMTPService, SMTPService>();
    }

    private static void AddUsuarioServices(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    }

    private static void AddClienteServices(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddSingleton<IValidator<CreateClienteRequest>, CreateClienteRequestValidator>();
        services.AddSingleton<IValidator<UpdateClienteRequest>, UpdateClienteRequestValidator>();
    }

    private static void AddVeiculoServices(this IServiceCollection services)
    {
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddSingleton<IValidator<CreateVeiculoRequest>, CreateVeiculoRequestValidator>();
        services.AddSingleton<IValidator<UpdateVeiculoRequest>, UpdateVeiculoRequestValidator>();
    }

    private static void AddServicoServices(this IServiceCollection services)
    {
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddSingleton<IValidator<CreateServicoRequest>, CreateServicoRequestValidator>();
        services.AddSingleton<IValidator<UpdateServicoRequest>, UpdateServicoRequestValidator>();
    }

    private static void AddItemEstoqueServices(this IServiceCollection services)
    {
        services.AddScoped<IItemEstoqueRepository, ItemEstoqueRepository>();
        services.AddSingleton<IValidator<CreateItemEstoqueRequest>, CreateItemEstoqueRequestValidator>();
        services.AddSingleton<IValidator<UpdateItemEstoqueRequest>, UpdateItemEstoqueRequestValidator>();
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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
    }
}
