using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class KeepStockAmountChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockAmount",
                table: "StockEntries");

            migrationBuilder.AddColumn<int>(
                name: "KeepStockAmount",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeepStockAmount",
                table: "ArticleUserSettings");

            migrationBuilder.AddColumn<int>(
                name: "StockAmount",
                table: "StockEntries",
                nullable: false,
                defaultValue: 0);
        }
    }
}
