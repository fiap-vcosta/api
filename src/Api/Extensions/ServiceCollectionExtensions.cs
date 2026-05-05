using Application.Usuario.Commands;
using Application.Services;
using Api.Validators;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Repositories;
using Infrastructure.Database;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IValidator<LoginRequest>, LoginRequestValidator>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
        services.AddControllers();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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
