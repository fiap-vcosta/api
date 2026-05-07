using Application.Abstractions.Services;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class HealthService(AppDbContext context) : IHealthService
{
    public async Task<bool> CheckDatabaseAsync()
    {
        try
        {
            await context.Usuarios.AnyAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}