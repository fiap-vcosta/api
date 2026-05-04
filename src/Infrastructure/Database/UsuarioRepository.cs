using Application.Repositories;
using Domain.Admin;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
{
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await context.Usuarios.FindAsync(id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await context.Usuarios.ToListAsync();
    }

    public async Task<Usuario?> GetByLoginAndPasswordAsync(string login, string password)
    {
        var hashedPassword = PasswordHasher.HashPassword(password);
        return await context.Usuarios.FirstOrDefaultAsync(u => u.Login == login && u.Password == hashedPassword);
    }
}