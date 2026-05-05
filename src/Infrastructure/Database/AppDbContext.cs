using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Login = "admin",
                Password = PasswordHasher.HashPassword("admin"),
                TipoUsuario = TipoUsuario.Admin
            }
        );
    }
}