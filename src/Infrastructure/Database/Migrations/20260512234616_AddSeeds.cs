using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicoItensNecessarios");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RejeitadoEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecucaoIniciadaEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecucaoFinalizadaEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AprovadoEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Documento", "Email", "Nome", "TipoDocumento" },
                values: new object[,]
                {
                    { 1, "43372251034", "joao.silva@email.com", "João Silva", "Cpf" },
                    { 2, "74694481024", "maria.oliveira@email.com", "Maria Oliveira", "Cpf" },
                    { 3, "31868352011", "carlos.santos@email.com", "Carlos Santos", "Cpf" },
                    { 4, "17293524021", "ana.costa@email.com", "Ana Costa", "Cpf" },
                    { 5, "84617462057", "lucas.ferreira@email.com", "Lucas Ferreira", "Cpf" },
                    { 6, "53820257000159", "contato@autopecascentral.com.br", "Auto Peças Central Ltda", "Cnpj" },
                    { 7, "19183615000195", "oficinatoninho@email.com", "Oficina do Toninho ME", "Cnpj" },
                    { 8, "75294528000185", "logistica@rapidosul.com.br", "Transportadora Rápido Sul", "Cnpj" },
                    { 9, "42168936000152", "frota@xyzlocadora.com.br", "Locadora de Veículos XYZ", "Cnpj" },
                    { 10, "85303964000182", "vendas@bateriaspotencia.com.br", "Comercial de Baterias Potência", "Cnpj" }
                });

            migrationBuilder.InsertData(
                table: "ItensEstoque",
                columns: new[] { "Id", "Codigo", "Nome", "PrecoVenda", "Saldo", "SaldoReservado", "Tipo", "UnidadeMedida" },
                values: new object[,]
                {
                    { 1, "INS-001", "Óleo Motor Sintético 5W30", 45.90m, 50.000m, 0m, "Insumo", "Litro" },
                    { 2, "INS-002", "Óleo Motor Semissintético 15W40", 38.50m, 0.000m, 0m, "Insumo", "Litro" },
                    { 3, "PEC-001", "Filtro de Óleo", 35.00m, 15.000m, 0m, "Peca", "Unidade" },
                    { 4, "PEC-002", "Filtro de Ar do Motor", 42.00m, 20.000m, 0m, "Peca", "Unidade" },
                    { 5, "PEC-003", "Filtro de Cabine (Ar Condicionado)", 30.00m, 0.000m, 0m, "Peca", "Unidade" },
                    { 6, "PEC-004", "Pastilha de Freio Dianteira", 120.00m, 8.000m, 0m, "Peca", "Jogo" },
                    { 7, "PEC-005", "Pastilha de Freio Traseira", 95.00m, 3.000m, 0m, "Peca", "Jogo" },
                    { 8, "INS-003", "Fluido de Freio DOT 4", 25.00m, 30.000m, 0m, "Insumo", "Frasco" },
                    { 9, "INS-004", "Chumbo para Balanceamento", 18.00m, 5.500m, 0m, "Insumo", "Kg" },
                    { 10, "INS-005", "Descarbonizante Spray", 22.00m, 10.000m, 0m, "Insumo", "Frasco" },
                    { 11, "PEC-006", "Kit O-ring Bico Injetor", 15.00m, 25.000m, 0m, "Peca", "Jogo" },
                    { 12, "PEC-016", "Correia Dentada", 85.00m, 6.000m, 0m, "Peca", "Unidade" },
                    { 13, "PEC-017", "Tensor da Correia Dentada", 110.00m, 4.000m, 0m, "Peca", "Unidade" },
                    { 14, "PEC-018", "Vela de Ignição", 140.00m, 0.000m, 0m, "Peca", "Jogo" },
                    { 15, "PEC-007", "Bateria 60Ah", 350.00m, 4.000m, 0m, "Peca", "Unidade" },
                    { 16, "PEC-008", "Bateria 45Ah", 280.00m, 0.000m, 0m, "Peca", "Unidade" },
                    { 17, "PEC-009", "Terminal de Bateria", 12.00m, 40.000m, 0m, "Peca", "Par" },
                    { 18, "PEC-014", "Lâmpada H4", 25.00m, 0.000m, 0m, "Peca", "Unidade" },
                    { 19, "PEC-015", "Lâmpada Pingo T10", 10.00m, 50.000m, 0m, "Peca", "Par" },
                    { 20, "INS-006", "Higienizador Granada", 28.00m, 18.000m, 0m, "Insumo", "Frasco" },
                    { 21, "INS-007", "Gás Refrigerante R134a", 85.00m, 3.200m, 0m, "Insumo", "Kg" },
                    { 22, "PEC-010", "Amortecedor Dianteiro", 450.00m, 2.000m, 0m, "Peca", "Par" },
                    { 23, "PEC-011", "Kit Batente Amortecedor Dianteiro", 80.00m, 5.000m, 0m, "Peca", "Jogo" },
                    { 24, "PEC-012", "Kit Embreagem Completo", 650.00m, 0.000m, 0m, "Peca", "Jogo" },
                    { 25, "INS-008", "Fluido de Transmissão Manual", 55.00m, 12.000m, 0m, "Insumo", "Litro" },
                    { 26, "PEC-013", "Palheta Limpador Parabrisa", 45.00m, 10.000m, 0m, "Peca", "Par" },
                    { 27, "INS-009", "Aditivo Radiador Concentrado", 32.00m, 24.000m, 0m, "Insumo", "Litro" },
                    { 28, "INS-010", "Água Desmineralizada", 8.00m, 60.000m, 0m, "Insumo", "Litro" },
                    { 29, "INS-011", "Estopa Polimento", 15.00m, 2.500m, 0m, "Insumo", "Kg" },
                    { 30, "INS-012", "Desengraxante Concentrado", 20.00m, 15.000m, 0m, "Insumo", "Litro" }
                });

            migrationBuilder.InsertData(
                table: "Servicos",
                columns: new[] { "Id", "Ativo", "Codigo", "Nome", "PrecoPadrao" },
                values: new object[,]
                {
                    { 1, true, "MTR-001", "Troca de Óleo do Motor", 150.00m },
                    { 2, true, "FLT-002", "Troca de Filtro de Ar", 50.00m },
                    { 3, true, "FRE-003", "Troca de Pastilhas de Freio", 200.00m },
                    { 4, true, "SUS-004", "Alinhamento e Balanceamento", 120.00m },
                    { 5, true, "MTR-005", "Limpeza de Bicos Injetores", 180.00m },
                    { 6, true, "ELT-006", "Troca de Bateria", 80.00m },
                    { 7, true, "ARC-007", "Higienização de Ar Condicionado", 130.00m },
                    { 8, true, "SUS-008", "Troca de Amortecedores Dianteiros", 300.00m },
                    { 9, true, "EMB-009", "Substituição do Kit de Embreagem", 600.00m },
                    { 10, true, "GRL-010", "Revisão Preventiva Geral", 250.00m }
                });

            migrationBuilder.InsertData(
                table: "Veiculos",
                columns: new[] { "Id", "IdDono", "Marca", "Modelo", "Placa" },
                values: new object[,]
                {
                    { 1, 1, "Chevrolet", "Onix", "ABC1234" },
                    { 2, 1, "Volkswagen", "Gol", "XYZ1A23" },
                    { 3, 1, "Honda", "Civic", "DEF5678" },
                    { 4, 2, "Toyota", "Corolla", "GHI9B12" },
                    { 5, 2, "Fiat", "Argo", "JKL3456" },
                    { 6, 3, "Hyundai", "HB20", "MNO4C56" },
                    { 7, 3, "Ford", "Ka", "PQR7890" },
                    { 8, 3, "Renault", "Kwid", "STU5D78" },
                    { 9, 4, "Jeep", "Compass", "VWX1234" },
                    { 10, 5, "Nissan", "Kicks", "YZA6E90" },
                    { 11, 5, "Chevrolet", "Tracker", "BCD5678" },
                    { 12, 6, "Volkswagen", "Saveiro", "EFG7F12" },
                    { 13, 6, "Fiat", "Fiorino", "HIJ9012" },
                    { 14, 6, "Peugeot", "Partner", "KLM8G34" },
                    { 15, 7, "Chevrolet", "Montana", "NOP3456" },
                    { 16, 8, "Mercedes-Benz", "Sprinter", "QRS9H56" },
                    { 17, 8, "Iveco", "Daily", "TUV7890" },
                    { 18, 8, "Ford", "Transit", "WXY0I78" },
                    { 19, 9, "Fiat", "Mobi", "ZAB1234" },
                    { 20, 10, "Volkswagen", "Kombi", "CDE1J90" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ItensEstoque",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RejeitadoEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecucaoIniciadaEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecucaoFinalizadaEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AprovadoEm",
                table: "ItensServicos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ServicoItensNecessarios",
                columns: table => new
                {
                    ServicoAggregateRootId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdItemEstoque = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoItensNecessarios", x => new { x.ServicoAggregateRootId, x.Id });
                    table.ForeignKey(
                        name: "FK_ServicoItensNecessarios_ItensEstoque_IdItemEstoque",
                        column: x => x.IdItemEstoque,
                        principalTable: "ItensEstoque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicoItensNecessarios_Servicos_ServicoAggregateRootId",
                        column: x => x.ServicoAggregateRootId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicoItensNecessarios_IdItemEstoque",
                table: "ServicoItensNecessarios",
                column: "IdItemEstoque");
        }
    }
}
