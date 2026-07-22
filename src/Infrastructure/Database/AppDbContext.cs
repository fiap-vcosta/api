using Domain.Administrativo.Entities;
using Domain.Estoque.Entities;
using Domain.OrdemServico.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        SeedClientes(clientes);

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
        SeedVeiculos(veiculos);

        // Serviços
        var servicos = modelBuilder.Entity<ServicoAggregateRoot>();
        servicos.HasKey(s => s.Id);
        servicos.HasIndex(s => s.Codigo).IsUnique();
        servicos.Property(s => s.Codigo).IsRequired();
        servicos.Property(s => s.Nome).IsRequired();
        servicos.Property(s => s.PrecoPadrao).HasPrecision(10, 2).IsRequired();
        SeedServicos(servicos);

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
        itensEstoque.Ignore(ie => ie.SaldoDisponivel);
        SeedItensEstoque(itensEstoque);

        // Ordens de Serviço
        var ordensServico = modelBuilder.Entity<OrdemServicoAggregateRoot>();
        ordensServico.HasKey(os => os.Id);
        ordensServico.HasIndex(os => os.Status);
        ordensServico.Property(os => os.Status).HasConversion<string>().IsRequired();
        ordensServico.Property(os => os.RecebidaEm).IsRequired();
        ordensServico.Property(os => os.EntregueEm);
        ordensServico.Property(os => os.DescartadaEm);
        ordensServico.HasIndex(os => os.TokenAprovacao).IsUnique();
        ordensServico.Property(os => os.TokenAprovacao).IsRequired();
        ordensServico.Ignore(os => os.ValorTotal);
        ordensServico.Ignore(os => os.ItensNecessariosParaExecucao);
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

    private static void SeedClientes(EntityTypeBuilder<ClienteAggregateRoot> clientes)
    {
        clientes.HasData(
            new ClienteAggregateRoot 
            { 
                Id = 1, 
                TipoDocumento = 0, 
                Documento = "43372251034", 
                Nome = "João Silva", 
                Email = "joao.silva@email.com" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 2, 
                TipoDocumento = 0, 
                Documento = "74694481024", 
                Nome = "Maria Oliveira", 
                Email = "maria.oliveira@email.com" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 3, 
                TipoDocumento = 0, 
                Documento = "31868352011", 
                Nome = "Carlos Santos", 
                Email = "carlos.santos@email.com" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 4, 
                TipoDocumento = 0, 
                Documento = "17293524021", 
                Nome = "Ana Costa", 
                Email = "ana.costa@email.com" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 5, 
                TipoDocumento = 0, 
                Documento = "84617462057", 
                Nome = "Lucas Ferreira", 
                Email = "lucas.ferreira@email.com" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 6, 
                TipoDocumento = (TipoDocumento)1, 
                Documento = "53820257000159", 
                Nome = "Auto Peças Central Ltda", 
                Email = "contato@autopecascentral.com.br" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 7, 
                TipoDocumento = (TipoDocumento)1,
                Documento = "19183615000195", 
                Nome = "Oficina do Toninho ME", 
                Email = "oficinatoninho@email.com" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 8, 
                TipoDocumento = (TipoDocumento)1,
                Documento = "75294528000185", 
                Nome = "Transportadora Rápido Sul", 
                Email = "logistica@rapidosul.com.br" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 9, 
                TipoDocumento = (TipoDocumento)1,
                Documento = "42168936000152", 
                Nome = "Locadora de Veículos XYZ", 
                Email = "frota@xyzlocadora.com.br" 
            },
            new ClienteAggregateRoot 
            { 
                Id = 10, 
                TipoDocumento = (TipoDocumento)1,
                Documento = "85303964000182", 
                Nome = "Comercial de Baterias Potência", 
                Email = "vendas@bateriaspotencia.com.br" 
            }
        );
    }

    private static void SeedVeiculos(EntityTypeBuilder<VeiculoAggregateRoot> veiculos)
    {
        veiculos.HasData(
            new VeiculoAggregateRoot { Id = 1, IdDono = 1, Placa = "ABC1234", Marca = "Chevrolet", Modelo = "Onix" },
            new VeiculoAggregateRoot { Id = 2, IdDono = 1, Placa = "XYZ1A23", Marca = "Volkswagen", Modelo = "Gol" },
            new VeiculoAggregateRoot { Id = 3, IdDono = 1, Placa = "DEF5678", Marca = "Honda", Modelo = "Civic" },
            new VeiculoAggregateRoot { Id = 4, IdDono = 2, Placa = "GHI9B12", Marca = "Toyota", Modelo = "Corolla" },
            new VeiculoAggregateRoot { Id = 5, IdDono = 2, Placa = "JKL3456", Marca = "Fiat", Modelo = "Argo" },
            new VeiculoAggregateRoot { Id = 6, IdDono = 3, Placa = "MNO4C56", Marca = "Hyundai", Modelo = "HB20" },
            new VeiculoAggregateRoot { Id = 7, IdDono = 3, Placa = "PQR7890", Marca = "Ford", Modelo = "Ka" },
            new VeiculoAggregateRoot { Id = 8, IdDono = 3, Placa = "STU5D78", Marca = "Renault", Modelo = "Kwid" },
            new VeiculoAggregateRoot { Id = 9, IdDono = 4, Placa = "VWX1234", Marca = "Jeep", Modelo = "Compass" },
            new VeiculoAggregateRoot { Id = 10, IdDono = 5, Placa = "YZA6E90", Marca = "Nissan", Modelo = "Kicks" },
            new VeiculoAggregateRoot { Id = 11, IdDono = 5, Placa = "BCD5678", Marca = "Chevrolet", Modelo = "Tracker" },
            new VeiculoAggregateRoot { Id = 12, IdDono = 6, Placa = "EFG7F12", Marca = "Volkswagen", Modelo = "Saveiro" },
            new VeiculoAggregateRoot { Id = 13, IdDono = 6, Placa = "HIJ9012", Marca = "Fiat", Modelo = "Fiorino" },
            new VeiculoAggregateRoot { Id = 14, IdDono = 6, Placa = "KLM8G34", Marca = "Peugeot", Modelo = "Partner" },
            new VeiculoAggregateRoot { Id = 15, IdDono = 7, Placa = "NOP3456", Marca = "Chevrolet", Modelo = "Montana" },
            new VeiculoAggregateRoot { Id = 16, IdDono = 8, Placa = "QRS9H56", Marca = "Mercedes-Benz", Modelo = "Sprinter" },
            new VeiculoAggregateRoot { Id = 17, IdDono = 8, Placa = "TUV7890", Marca = "Iveco", Modelo = "Daily" },
            new VeiculoAggregateRoot { Id = 18, IdDono = 8, Placa = "WXY0I78", Marca = "Ford", Modelo = "Transit" },
            new VeiculoAggregateRoot { Id = 19, IdDono = 9, Placa = "ZAB1234", Marca = "Fiat", Modelo = "Mobi" },
            new VeiculoAggregateRoot { Id = 20, IdDono = 10, Placa = "CDE1J90", Marca = "Volkswagen", Modelo = "Kombi" }
        );
    }

    private static void SeedServicos(EntityTypeBuilder<ServicoAggregateRoot> servicos)
    {
        servicos.HasData(
            new ServicoAggregateRoot { Id = 1, Codigo = "MTR-001", Nome = "Troca de Óleo do Motor", PrecoPadrao = 150.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 2, Codigo = "FLT-002", Nome = "Troca de Filtro de Ar", PrecoPadrao = 50.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 3, Codigo = "FRE-003", Nome = "Troca de Pastilhas de Freio", PrecoPadrao = 200.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 4, Codigo = "SUS-004", Nome = "Alinhamento e Balanceamento", PrecoPadrao = 120.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 5, Codigo = "MTR-005", Nome = "Limpeza de Bicos Injetores", PrecoPadrao = 180.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 6, Codigo = "ELT-006", Nome = "Troca de Bateria", PrecoPadrao = 80.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 7, Codigo = "ARC-007", Nome = "Higienização de Ar Condicionado", PrecoPadrao = 130.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 8, Codigo = "SUS-008", Nome = "Troca de Amortecedores Dianteiros", PrecoPadrao = 300.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 9, Codigo = "EMB-009", Nome = "Substituição do Kit de Embreagem", PrecoPadrao = 600.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 10, Codigo = "GRL-010", Nome = "Revisão Preventiva Geral", PrecoPadrao = 250.00m, Ativo = true }
        );
    }

    private static void SeedItensEstoque(EntityTypeBuilder<ItemEstoqueAggregateRoot> itensEstoque)
    {
        itensEstoque.HasData(
            new ItemEstoqueAggregateRoot { Id = 1, Codigo = "INS-001", Tipo = ItemTipo.Insumo, Nome = "Óleo Motor Sintético 5W30", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 45.90m, Saldo = 50.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 2, Codigo = "INS-002", Tipo = ItemTipo.Insumo, Nome = "Óleo Motor Semissintético 15W40", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 38.50m, Saldo = 0.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 3, Codigo = "PEC-001", Tipo = ItemTipo.Peca, Nome = "Filtro de Óleo", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 35.00m, Saldo = 15.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 4, Codigo = "PEC-002", Tipo = ItemTipo.Peca, Nome = "Filtro de Ar do Motor", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 42.00m, Saldo = 20.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 5, Codigo = "PEC-003", Tipo = ItemTipo.Peca, Nome = "Filtro de Cabine (Ar Condicionado)", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 30.00m, Saldo = 0.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 6, Codigo = "PEC-004", Tipo = ItemTipo.Peca, Nome = "Pastilha de Freio Dianteira", UnidadeMedida = UnidadeMedida.Jogo, PrecoVenda = 120.00m, Saldo = 8.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 7, Codigo = "PEC-005", Tipo = ItemTipo.Peca, Nome = "Pastilha de Freio Traseira", UnidadeMedida = UnidadeMedida.Jogo, PrecoVenda = 95.00m, Saldo = 3.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 8, Codigo = "INS-003", Tipo = ItemTipo.Insumo, Nome = "Fluido de Freio DOT 4", UnidadeMedida = UnidadeMedida.Frasco, PrecoVenda = 25.00m, Saldo = 30.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 9, Codigo = "INS-004", Tipo = ItemTipo.Insumo, Nome = "Chumbo para Balanceamento", UnidadeMedida = UnidadeMedida.Kg, PrecoVenda = 18.00m, Saldo = 5.500m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 10, Codigo = "INS-005", Tipo = ItemTipo.Insumo, Nome = "Descarbonizante Spray", UnidadeMedida = UnidadeMedida.Frasco, PrecoVenda = 22.00m, Saldo = 10.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 11, Codigo = "PEC-006", Tipo = ItemTipo.Peca, Nome = "Kit O-ring Bico Injetor", UnidadeMedida = UnidadeMedida.Jogo, PrecoVenda = 15.00m, Saldo = 25.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 12, Codigo = "PEC-016", Tipo = ItemTipo.Peca, Nome = "Correia Dentada", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 85.00m, Saldo = 6.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 13, Codigo = "PEC-017", Tipo = ItemTipo.Peca, Nome = "Tensor da Correia Dentada", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 110.00m, Saldo = 4.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 14, Codigo = "PEC-018", Tipo = ItemTipo.Peca, Nome = "Vela de Ignição", UnidadeMedida = UnidadeMedida.Jogo, PrecoVenda = 140.00m, Saldo = 0.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 15, Codigo = "PEC-007", Tipo = ItemTipo.Peca, Nome = "Bateria 60Ah", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 350.00m, Saldo = 4.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 16, Codigo = "PEC-008", Tipo = ItemTipo.Peca, Nome = "Bateria 45Ah", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 280.00m, Saldo = 0.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 17, Codigo = "PEC-009", Tipo = ItemTipo.Peca, Nome = "Terminal de Bateria", UnidadeMedida = UnidadeMedida.Par, PrecoVenda = 12.00m, Saldo = 40.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 18, Codigo = "PEC-014", Tipo = ItemTipo.Peca, Nome = "Lâmpada H4", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 25.00m, Saldo = 0.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 19, Codigo = "PEC-015", Tipo = ItemTipo.Peca, Nome = "Lâmpada Pingo T10", UnidadeMedida = UnidadeMedida.Par, PrecoVenda = 10.00m, Saldo = 50.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 20, Codigo = "INS-006", Tipo = ItemTipo.Insumo, Nome = "Higienizador Granada", UnidadeMedida = UnidadeMedida.Frasco, PrecoVenda = 28.00m, Saldo = 18.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 21, Codigo = "INS-007", Tipo = ItemTipo.Insumo, Nome = "Gás Refrigerante R134a", UnidadeMedida = UnidadeMedida.Kg, PrecoVenda = 85.00m, Saldo = 3.200m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 22, Codigo = "PEC-010", Tipo = ItemTipo.Peca, Nome = "Amortecedor Dianteiro", UnidadeMedida = UnidadeMedida.Par, PrecoVenda = 450.00m, Saldo = 2.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 23, Codigo = "PEC-011", Tipo = ItemTipo.Peca, Nome = "Kit Batente Amortecedor Dianteiro", UnidadeMedida = UnidadeMedida.Jogo, PrecoVenda = 80.00m, Saldo = 5.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 24, Codigo = "PEC-012", Tipo = ItemTipo.Peca, Nome = "Kit Embreagem Completo", UnidadeMedida = UnidadeMedida.Jogo, PrecoVenda = 650.00m, Saldo = 0.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 25, Codigo = "INS-008", Tipo = ItemTipo.Insumo, Nome = "Fluido de Transmissão Manual", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 55.00m, Saldo = 12.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 26, Codigo = "PEC-013", Tipo = ItemTipo.Peca, Nome = "Palheta Limpador Parabrisa", UnidadeMedida = UnidadeMedida.Par, PrecoVenda = 45.00m, Saldo = 10.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 27, Codigo = "INS-009", Tipo = ItemTipo.Insumo, Nome = "Aditivo Radiador Concentrado", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 32.00m, Saldo = 24.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 28, Codigo = "INS-010", Tipo = ItemTipo.Insumo, Nome = "Água Desmineralizada", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 8.00m, Saldo = 60.000m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 29, Codigo = "INS-011", Tipo = ItemTipo.Insumo, Nome = "Estopa Polimento", UnidadeMedida = UnidadeMedida.Kg, PrecoVenda = 15.00m, Saldo = 2.500m, SaldoReservado = 0m },
            new ItemEstoqueAggregateRoot { Id = 30, Codigo = "INS-012", Tipo = ItemTipo.Insumo, Nome = "Desengraxante Concentrado", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 20.00m, Saldo = 15.000m, SaldoReservado = 0m }
        );
    }
}