using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Clientes_DonoId",
                table: "Veiculos");

            migrationBuilder.DropTable(
                name: "ServicoItemEstoque");

            migrationBuilder.RenameColumn(
                name: "DonoId",
                table: "Veiculos",
                newName: "IdDono");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculos_DonoId",
                table: "Veiculos",
                newName: "IX_Veiculos_IdDono");

            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadeMedida",
                table: "ItensEstoque",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "ItensEstoque",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDocumento",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "OrdensServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RecebidaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntregueEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DescartadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Cliente_Email = table.Column<string>(type: "text", nullable: false),
                    Cliente_Nome = table.Column<string>(type: "text", nullable: false),
                    Veiculo_Marca = table.Column<string>(type: "text", nullable: false),
                    Veiculo_Modelo = table.Column<string>(type: "text", nullable: false),
                    Veiculo_Placa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "ItemOrdemServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    ValorCobrado = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOrdemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOrdemServico_OrdensServico_IdOrdemServico",
                        column: x => x.IdOrdemServico,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemEstoqueOrdemServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    IdItemOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    UnidadeMedida = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEstoqueOrdemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemEstoqueOrdemServico_ItemOrdemServico_IdItemOrdemServico",
                        column: x => x.IdItemOrdemServico,
                        principalTable: "ItemOrdemServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemEstoqueOrdemServico_OrdensServico_IdOrdemServico",
                        column: x => x.IdOrdemServico,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "TipoUsuario",
                value: "Admin");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoDocumento",
                table: "Clientes",
                column: "TipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_ItemEstoqueOrdemServico_IdItemOrdemServico",
                table: "ItemEstoqueOrdemServico",
                column: "IdItemOrdemServico");

            migrationBuilder.CreateIndex(
                name: "IX_ItemEstoqueOrdemServico_IdOrdemServico",
                table: "ItemEstoqueOrdemServico",
                column: "IdOrdemServico");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrdemServico_IdOrdemServico",
                table: "ItemOrdemServico",
                column: "IdOrdemServico");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrdemServico_Status",
                table: "ItemOrdemServico",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Status",
                table: "OrdensServico",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServicoItensNecessarios_IdItemEstoque",
                table: "ServicoItensNecessarios",
                column: "IdItemEstoque");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Clientes_IdDono",
                table: "Veiculos",
                column: "IdDono",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Clientes_IdDono",
                table: "Veiculos");

            migrationBuilder.DropTable(
                name: "ItemEstoqueOrdemServico");

            migrationBuilder.DropTable(
                name: "ServicoItensNecessarios");

            migrationBuilder.DropTable(
                name: "ItemOrdemServico");

            migrationBuilder.DropTable(
                name: "OrdensServico");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_TipoDocumento",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "IdDono",
                table: "Veiculos",
                newName: "DonoId");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculos_IdDono",
                table: "Veiculos",
                newName: "IX_Veiculos_DonoId");

            migrationBuilder.AlterColumn<int>(
                name: "TipoUsuario",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UnidadeMedida",
                table: "ItensEstoque",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "ItensEstoque",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "TipoDocumento",
                table: "Clientes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ServicoItemEstoque",
                columns: table => new
                {
                    ServicoId = table.Column<int>(type: "integer", nullable: false),
                    ItemEstoqueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoItemEstoque", x => new { x.ServicoId, x.ItemEstoqueId });
                    table.ForeignKey(
                        name: "FK_ServicoItemEstoque_ItensEstoque_ItemEstoqueId",
                        column: x => x.ItemEstoqueId,
                        principalTable: "ItensEstoque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicoItemEstoque_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "TipoUsuario",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServicoItemEstoque_ItemEstoqueId",
                table: "ServicoItemEstoque",
                column: "ItemEstoqueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Clientes_DonoId",
                table: "Veiculos",
                column: "DonoId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
