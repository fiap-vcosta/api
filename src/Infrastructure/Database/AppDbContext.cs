using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Servico> Servicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>().HasIndex(c => c.Documento).IsUnique();
        modelBuilder.Entity<Cliente>().Property(c => c.Documento).IsRequired();

        modelBuilder.Entity<Veiculo>().HasIndex(v => v.Placa).IsUnique();
        modelBuilder.Entity<Veiculo>().Property(v => v.Placa).IsRequired();
        modelBuilder.Entity<Veiculo>().Property(v => v.Modelo).IsRequired();
        modelBuilder.Entity<Veiculo>().Property(v => v.Marca).IsRequired();
        modelBuilder.Entity<Veiculo>()
            .HasOne(v => v.Dono)
            .WithMany()
            .HasForeignKey(v => v.DonoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Servico>().HasIndex(s => s.Codigo).IsUnique();
        modelBuilder.Entity<Servico>().Property(s => s.Codigo).IsRequired();
        modelBuilder.Entity<Servico>().Property(s => s.Nome).IsRequired();
        modelBuilder.Entity<Servico>().Property(s => s.PrecoPadrao).HasPrecision(10, 2).IsRequired();

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