using Domain.Administrativo.Entities;
using Domain.Estoque.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Servico> Servicos { get; set; }
    public DbSet<ItemEstoque> ItensEstoque { get; set; }

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
        modelBuilder.Entity<Servico>()
            .HasMany(s => s.ItensNecessarios)
            .WithMany(i => i.Servicos)
            .UsingEntity<Dictionary<string, object>>(
                "ServicoItemEstoque",
                j => j.HasOne<ItemEstoque>().WithMany().HasForeignKey("ItemEstoqueId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Servico>().WithMany().HasForeignKey("ServicoId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("ServicoId", "ItemEstoqueId")
            );

        modelBuilder.Entity<ItemEstoque>().HasIndex(i => i.Codigo).IsUnique();
        modelBuilder.Entity<ItemEstoque>().Property(i => i.Codigo).IsRequired();
        modelBuilder.Entity<ItemEstoque>().Property(i => i.Nome).IsRequired();
        modelBuilder.Entity<ItemEstoque>().Property(i => i.PrecoVenda).HasPrecision(10, 2).IsRequired();
        modelBuilder.Entity<ItemEstoque>().Property(i => i.Saldo).HasPrecision(10, 3).IsRequired();
        modelBuilder.Entity<ItemEstoque>().Property(i => i.SaldoReservado).HasPrecision(10, 3).IsRequired();

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