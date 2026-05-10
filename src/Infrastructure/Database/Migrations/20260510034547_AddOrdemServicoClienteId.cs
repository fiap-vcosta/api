using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServicoClienteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cliente_Id",
                table: "OrdensServico",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejeitadaEm",
                table: "OrdensServico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AprovadoEm",
                table: "ItemOrdemServico",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RejeitadoEm",
                table: "ItemOrdemServico",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cliente_Id",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "RejeitadaEm",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "AprovadoEm",
                table: "ItemOrdemServico");

            migrationBuilder.DropColumn(
                name: "RejeitadoEm",
                table: "ItemOrdemServico");
        }
    }
}
