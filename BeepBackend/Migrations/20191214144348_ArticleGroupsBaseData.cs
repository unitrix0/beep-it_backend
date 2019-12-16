using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class ArticleGroupsBaseData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ArticleGroups",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "keine" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ArticleGroups",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
