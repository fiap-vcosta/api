using Application.Abstractions.Gateways;
using Domain.Administrativo.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Gateways;

public class UsuarioGateway(AppDbContext context) : IUsuarioGateway
{
    public async Task<UsuarioAggregateRoot?> GetByIdAsync(int id)
    {
        return await context.Usuarios.FindAsync(id);
    }

    public async Task<IEnumerable<UsuarioAggregateRoot>> GetAllAsync()
    {
        return await context.Usuarios.ToListAsync();
    }

    public async Task<UsuarioAggregateRoot?> GetByLoginAndSenhaAsync(string login, string senha)
    {
        var hashedSenha = PasswordHasher.HashPassword(senha);
        return await context.Usuarios.FirstOrDefaultAsync(u => u.Login == login && u.Senha == hashedSenha);
    }
}
