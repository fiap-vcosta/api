using Domain.Administrativo.Entities;
using Domain.Estoque.Entities;
using Domain.OrdemServico.Entities;
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
    public DbSet<OrdemServicoAggregateRoot> OrdensServico { get; set; }
    public DbSet<Servico> ItensServicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Usuários
        var usuarios = modelBuilder.Entity<UsuarioAggregateRoot>();
        usuarios.HasKey(u => u.Id);
        usuarios.HasIndex(u => u.Login).IsUnique();
        usuarios.Property(u => u.Login).IsRequired();
        usuarios.Property(u => u.Password).IsRequired();
        usuarios.Property(u => u.TipoUsuario).HasConversion<string>().IsRequired();
        usuarios.HasData(
            new UsuarioAggregateRoot
            {
                Id = 1,
                Login = "admin",
                Password = PasswordHasher.HashPassword("admin"),
                TipoUsuario = TipoUsuario.Admin
            }
        );

        // Clientes
        var clientes = modelBuilder.Entity<ClienteAggregateRoot>();
        clientes.HasKey(c => c.Id);
        clientes.HasIndex(c => c.TipoDocumento);
        clientes.Property(c => c.TipoDocumento).HasConversion<string>().IsRequired();
        clientes.HasIndex(c => c.Documento).IsUnique();
        clientes.Property(c => c.Documento).IsRequired();
        clientes.Property(c => c.Nome).IsRequired();
        clientes.Property(c => c.Email).IsRequired();

        // Veiculos
        var veiculos = modelBuilder.Entity<VeiculoAggregateRoot>();
        veiculos.HasKey(v => v.Id);
        veiculos.HasIndex(v => v.Placa).IsUnique();
        veiculos.Property(v => v.Placa).IsRequired();
        veiculos.Property(v => v.Modelo).IsRequired();
        veiculos.Property(v => v.Marca).IsRequired();
        veiculos
            .HasOne<ClienteAggregateRoot>()
            .WithMany()
            .HasForeignKey(v => v.IdDono)
            .OnDelete(DeleteBehavior.Restrict);

        // Serviços
        var servicos = modelBuilder.Entity<ServicoAggregateRoot>();
        servicos.HasKey(s => s.Id);
        servicos.HasIndex(s => s.Codigo).IsUnique();
        servicos.Property(s => s.Codigo).IsRequired();
        servicos.Property(s => s.Nome).IsRequired();
        servicos.Property(s => s.PrecoPadrao).HasPrecision(10, 2).IsRequired();
        servicos
            .OwnsMany(s => s.ItensNecessarios, item =>
            {
                item.ToTable("ServicoItensNecessarios");
                item.HasOne<ItemEstoqueAggregateRoot>()
                    .WithMany()
                    .HasForeignKey(i => i.IdItemEstoque)
                    .OnDelete(DeleteBehavior.Restrict);

            });

        // Itens Estoque
        var itensEstoque = modelBuilder.Entity<ItemEstoqueAggregateRoot>();
        itensEstoque.HasKey(ie => ie.Id);
        itensEstoque.HasIndex(ie => ie.Codigo).IsUnique();
        itensEstoque.Property(ie => ie.Codigo).IsRequired();
        itensEstoque.Property(ie => ie.Tipo).HasConversion<string>().IsRequired();
        itensEstoque.Property(ie => ie.Nome).IsRequired();
        itensEstoque.Property(ie => ie.UnidadeMedida).HasConversion<string>().IsRequired();
        itensEstoque.Property(ie => ie.PrecoVenda).HasPrecision(10, 2).IsRequired();
        itensEstoque.Property(ie => ie.Saldo).HasPrecision(10, 3).IsRequired();
        itensEstoque.Property(ie => ie.SaldoReservado).HasPrecision(10, 3).IsRequired();

        // Ordens de Serviço
        var ordensServico = modelBuilder.Entity<OrdemServicoAggregateRoot>();
        ordensServico.HasKey(os => os.Id);
        ordensServico.HasIndex(os => os.Status);
        ordensServico.Property(os => os.Status).HasConversion<string>().IsRequired();
        ordensServico.Property(os => os.RecebidaEm).IsRequired();
        ordensServico.Property(os => os.EntregueEm);
        ordensServico.Property(os => os.DescartadaEm);
        ordensServico.ComplexProperty(os => os.Cliente);
        ordensServico.ComplexProperty(os => os.Veiculo);
        ordensServico
            .HasMany(os => os.Servicos)
            .WithOne()
            .HasForeignKey(ios => ios.IdOrdemServico)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Itens de Serviço Ordem de Serviço
        var servicosOrdemServico = modelBuilder.Entity<Servico>();
        servicosOrdemServico.HasKey(ios => ios.Id);
        servicosOrdemServico.HasIndex(ios => ios.Status);
        servicosOrdemServico.Property(ios => ios.Status).HasConversion<string>().IsRequired();
        servicosOrdemServico.Property(ios => ios.Nome).IsRequired();
        servicosOrdemServico.Property(ios => ios.ValorCobrado).HasPrecision(10, 2).IsRequired();
        servicosOrdemServico.ComplexProperty(ios => ios.ServicoCatalogo);
        servicosOrdemServico
            .HasMany(ios => ios.ItensNecessarios)
            .WithOne()
            .HasForeignKey(ieos => ieos.IdItemOrdemServico)
            .OnDelete(DeleteBehavior.Cascade);

        // Itens de Estoque Ordem de Serviço
        var itensNecessariosOrdemServico = modelBuilder.Entity<ItemNecessario>();
        itensNecessariosOrdemServico.HasKey(inos => inos.Id);
        itensNecessariosOrdemServico.HasIndex(inos => inos.Status);
        itensNecessariosOrdemServico.Property(inos => inos.Status).HasConversion<string>().IsRequired();
        itensNecessariosOrdemServico.Property(inos => inos.Quantidade).HasPrecision(10, 3).IsRequired();
        itensNecessariosOrdemServico.ComplexProperty(inos => inos.ItemEstoque);
        itensNecessariosOrdemServico
            .HasOne<OrdemServicoAggregateRoot>()
            .WithMany()
            .HasForeignKey(ie => ie.IdOrdemServico)
            .OnDelete(DeleteBehavior.Restrict);
    }
}