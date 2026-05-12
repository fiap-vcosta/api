using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddItensServicoCatalogoValueOBject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemNecessario_OrdensServico_OrdemServicoAggregateRootId",
                table: "ItemNecessario");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemNecessario_Servico_IdItemOrdemServico",
                table: "ItemNecessario");

            migrationBuilder.DropForeignKey(
                name: "FK_Servico_OrdensServico_IdOrdemServico",
                table: "Servico");

            migrationBuilder.DropIndex(
                name: "IX_ItemNecessario_OrdemServicoAggregateRootId",
                table: "ItemNecessario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Servico",
                table: "Servico");

            migrationBuilder.DropColumn(
                name: "OrdemServicoAggregateRootId",
                table: "ItemNecessario");

            migrationBuilder.RenameTable(
                name: "Servico",
                newName: "ItensServicos");

            migrationBuilder.RenameIndex(
                name: "IX_Servico_Status",
                table: "ItensServicos",
                newName: "IX_ItensServicos_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Servico_IdOrdemServico",
                table: "ItensServicos",
                newName: "IX_ItensServicos_IdOrdemServico");

            migrationBuilder.AddColumn<string>(
                name: "ServicoCatalogo_Codigo",
                table: "ItensServicos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ServicoCatalogo_Id",
                table: "ItensServicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServicoCatalogo_Nome",
                table: "ItensServicos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItensServicos",
                table: "ItensServicos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemNecessario_ItensServicos_IdItemOrdemServico",
                table: "ItemNecessario",
                column: "IdItemOrdemServico",
                principalTable: "ItensServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensServicos_OrdensServico_IdOrdemServico",
                table: "ItensServicos",
                column: "IdOrdemServico",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemNecessario_ItensServicos_IdItemOrdemServico",
                table: "ItemNecessario");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensServicos_OrdensServico_IdOrdemServico",
                table: "ItensServicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItensServicos",
                table: "ItensServicos");

            migrationBuilder.DropColumn(
                name: "ServicoCatalogo_Codigo",
                table: "ItensServicos");

            migrationBuilder.DropColumn(
                name: "ServicoCatalogo_Id",
                table: "ItensServicos");

            migrationBuilder.DropColumn(
                name: "ServicoCatalogo_Nome",
                table: "ItensServicos");

            migrationBuilder.RenameTable(
                name: "ItensServicos",
                newName: "Servico");

            migrationBuilder.RenameIndex(
                name: "IX_ItensServicos_Status",
                table: "Servico",
                newName: "IX_Servico_Status");

            migrationBuilder.RenameIndex(
                name: "IX_ItensServicos_IdOrdemServico",
                table: "Servico",
                newName: "IX_Servico_IdOrdemServico");

            migrationBuilder.AddColumn<int>(
                name: "OrdemServicoAggregateRootId",
                table: "ItemNecessario",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Servico",
                table: "Servico",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNecessario_OrdemServicoAggregateRootId",
                table: "ItemNecessario",
                column: "OrdemServicoAggregateRootId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemNecessario_OrdensServico_OrdemServicoAggregateRootId",
                table: "ItemNecessario",
                column: "OrdemServicoAggregateRootId",
                principalTable: "OrdensServico",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemNecessario_Servico_IdItemOrdemServico",
                table: "ItemNecessario",
                column: "IdItemOrdemServico",
                principalTable: "Servico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servico_OrdensServico_IdOrdemServico",
                table: "Servico",
                column: "IdOrdemServico",
                principalTable: "OrdensServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
