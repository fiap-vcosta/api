using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrdemServicoStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemEstoqueOrdemServico");

            migrationBuilder.DropTable(
                name: "ItemOrdemServico");

            migrationBuilder.AddColumn<DateTime>(
                name: "AprovadaEm",
                table: "OrdensServico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Servico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AprovadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RejeitadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecucaoIniciadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecucaoFinalizadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    ValorCobrado = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servico_OrdensServico_IdOrdemServico",
                        column: x => x.IdOrdemServico,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemNecessario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    IdItemOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    OrdemServicoAggregateRootId = table.Column<int>(type: "integer", nullable: true),
                    ItemEstoque_Codigo = table.Column<string>(type: "text", nullable: false),
                    ItemEstoque_Id = table.Column<int>(type: "integer", nullable: false),
                    ItemEstoque_Nome = table.Column<string>(type: "text", nullable: false),
                    ItemEstoque_UnidadeMedida = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemNecessario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemNecessario_OrdensServico_IdOrdemServico",
                        column: x => x.IdOrdemServico,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemNecessario_OrdensServico_OrdemServicoAggregateRootId",
                        column: x => x.OrdemServicoAggregateRootId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemNecessario_Servico_IdItemOrdemServico",
                        column: x => x.IdItemOrdemServico,
                        principalTable: "Servico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemNecessario_IdItemOrdemServico",
                table: "ItemNecessario",
                column: "IdItemOrdemServico");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNecessario_IdOrdemServico",
                table: "ItemNecessario",
                column: "IdOrdemServico");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNecessario_OrdemServicoAggregateRootId",
                table: "ItemNecessario",
                column: "OrdemServicoAggregateRootId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNecessario_Status",
                table: "ItemNecessario",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_IdOrdemServico",
                table: "Servico",
                column: "IdOrdemServico");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_Status",
                table: "Servico",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemNecessario");

            migrationBuilder.DropTable(
                name: "Servico");

            migrationBuilder.DropColumn(
                name: "AprovadaEm",
                table: "OrdensServico");

            migrationBuilder.CreateTable(
                name: "ItemOrdemServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AprovadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    RejeitadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
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
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    IdItemOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    IdOrdemServico = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    UnidadeMedida = table.Column<string>(type: "text", nullable: false)
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
        }
    }
}
