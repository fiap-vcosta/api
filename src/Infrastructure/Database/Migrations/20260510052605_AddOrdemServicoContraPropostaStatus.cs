using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServicoContraPropostaStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejeitadaEm",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "OrdensServico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejeitadaEm",
                table: "OrdensServico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "OrdensServico",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
