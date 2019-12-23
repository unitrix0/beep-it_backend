using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RemoveUnitOnArticle2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleUnits_ArticleUnitId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleUnitId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleUnitId",
                table: "Articles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleUnitId",
                table: "Articles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleUnitId",
                table: "Articles",
                column: "ArticleUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleUnits_ArticleUnitId",
                table: "Articles",
                column: "ArticleUnitId",
                principalTable: "ArticleUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
