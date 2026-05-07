using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

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