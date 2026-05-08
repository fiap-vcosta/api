using Domain.Administrativo.Entities;
using Domain.Estoque.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UsuarioAggregateRoot> Usuarios { get; set; }
    public DbSet<ClienteAggregateRoot> Clientes { get; set; }
    public DbSet<VeiculoAggregateRoot> Veiculos { get; set; }
    public DbSet<ServicoAggregateRoot> Servicos { get; set; }
    public DbSet<ItemEstoqueAggregateRoot> ItensEstoque { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClienteAggregateRoot>().HasIndex(c => c.Documento).IsUnique();
        modelBuilder.Entity<ClienteAggregateRoot>().Property(c => c.Documento).IsRequired();

        modelBuilder.Entity<VeiculoAggregateRoot>().HasIndex(v => v.Placa).IsUnique();
        modelBuilder.Entity<VeiculoAggregateRoot>().Property(v => v.Placa).IsRequired();
        modelBuilder.Entity<VeiculoAggregateRoot>().Property(v => v.Modelo).IsRequired();
        modelBuilder.Entity<VeiculoAggregateRoot>().Property(v => v.Marca).IsRequired();
        modelBuilder.Entity<VeiculoAggregateRoot>()
            .HasOne(v => v.Dono)
            .WithMany()
            .HasForeignKey(v => v.DonoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServicoAggregateRoot>().HasIndex(s => s.Codigo).IsUnique();
        modelBuilder.Entity<ServicoAggregateRoot>().Property(s => s.Codigo).IsRequired();
        modelBuilder.Entity<ServicoAggregateRoot>().Property(s => s.Nome).IsRequired();
        modelBuilder.Entity<ServicoAggregateRoot>().Property(s => s.PrecoPadrao).HasPrecision(10, 2).IsRequired();
        modelBuilder.Entity<ServicoAggregateRoot>()
            .HasMany(s => s.ItensNecessarios)
            .WithMany(i => i.Servicos)
            .UsingEntity<Dictionary<string, object>>(
                "ServicoItemEstoque",
                j => j.HasOne<ItemEstoqueAggregateRoot>().WithMany().HasForeignKey("ItemEstoqueId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<ServicoAggregateRoot>().WithMany().HasForeignKey("ServicoId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("ServicoId", "ItemEstoqueId")
            );

        modelBuilder.Entity<ItemEstoqueAggregateRoot>().HasIndex(i => i.Codigo).IsUnique();
        modelBuilder.Entity<ItemEstoqueAggregateRoot>().Property(i => i.Codigo).IsRequired();
        modelBuilder.Entity<ItemEstoqueAggregateRoot>().Property(i => i.Nome).IsRequired();
        modelBuilder.Entity<ItemEstoqueAggregateRoot>().Property(i => i.PrecoVenda).HasPrecision(10, 2).IsRequired();
        modelBuilder.Entity<ItemEstoqueAggregateRoot>().Property(i => i.Saldo).HasPrecision(10, 3).IsRequired();
        modelBuilder.Entity<ItemEstoqueAggregateRoot>().Property(i => i.SaldoReservado).HasPrecision(10, 3).IsRequired();

        modelBuilder.Entity<UsuarioAggregateRoot>().HasData(
            new UsuarioAggregateRoot
            {
                Id = 1,
                Login = "admin",
                Password = PasswordHasher.HashPassword("admin"),
                TipoUsuario = TipoUsuario.Admin
            }
        );
    }
}