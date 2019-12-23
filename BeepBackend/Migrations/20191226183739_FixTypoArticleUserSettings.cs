using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class FixTypoArticleUserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeppStockMode",
                table: "ArticleUserSettings",
                newName: "KeepStockMode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeepStockMode",
                table: "ArticleUserSettings",
                newName: "KeppStockMode");
        }
    }
}
