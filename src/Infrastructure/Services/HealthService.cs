using Application;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class HealthService : IHealthService
{
    private readonly AppDbContext _context;

    public HealthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckDatabaseAsync()
    {
        try
        {
            await _context.Usuarios.AnyAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}