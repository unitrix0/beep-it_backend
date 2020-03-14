using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class NewArticleUnitMilliliter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ArticleUnits",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[] { 7, "ml", "Milliliter" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ArticleUnits",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
