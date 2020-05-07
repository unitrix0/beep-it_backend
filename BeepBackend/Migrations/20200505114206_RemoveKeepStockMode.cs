using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RemoveKeepStockMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeepStockMode",
                table: "ArticleUserSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeepStockMode",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);
        }
    }
}
