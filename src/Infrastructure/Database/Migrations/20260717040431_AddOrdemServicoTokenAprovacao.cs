using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServicoTokenAprovacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenAprovacao",
                table: "OrdensServico",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "OrdensServico"
                SET "TokenAprovacao" = md5(random()::text || "Id"::text || clock_timestamp()::text)
                WHERE "TokenAprovacao" IS NULL OR "TokenAprovacao" = '';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "TokenAprovacao",
                table: "OrdensServico",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_TokenAprovacao",
                table: "OrdensServico",
                column: "TokenAprovacao",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrdensServico_TokenAprovacao",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "TokenAprovacao",
                table: "OrdensServico");
        }
    }
}
