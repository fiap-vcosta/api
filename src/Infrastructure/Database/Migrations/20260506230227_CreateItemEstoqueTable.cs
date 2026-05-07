using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateItemEstoqueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItensEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    UnidadeMedida = table.Column<int>(type: "integer", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Saldo = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    SaldoReservado = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensEstoque", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ItensEstoque_Codigo",
                table: "ItensEstoque",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicoItemEstoque_ItemEstoqueId",
                table: "ServicoItemEstoque",
                column: "ItemEstoqueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicoItemEstoque");

            migrationBuilder.DropTable(
                name: "ItensEstoque");
        }
    }
}
