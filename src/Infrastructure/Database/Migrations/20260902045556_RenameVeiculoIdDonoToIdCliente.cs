using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameVeiculoIdDonoToIdCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Clientes_IdDono",
                table: "Veiculos");

            migrationBuilder.RenameColumn(
                name: "IdDono",
                table: "Veiculos",
                newName: "IdCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculos_IdDono",
                table: "Veiculos",
                newName: "IX_Veiculos_IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Clientes_IdCliente",
                table: "Veiculos",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Clientes_IdCliente",
                table: "Veiculos");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "Veiculos",
                newName: "IdDono");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculos_IdCliente",
                table: "Veiculos",
                newName: "IX_Veiculos_IdDono");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Clientes_IdDono",
                table: "Veiculos",
                column: "IdDono",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
