using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastructure;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Login == login);
    }
}