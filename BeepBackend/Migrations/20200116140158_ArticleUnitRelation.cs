using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class ArticleUnitRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Articles",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UnitId",
                table: "Articles",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleUnits_UnitId",
                table: "Articles",
                column: "UnitId",
                principalTable: "ArticleUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleUnits_UnitId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_UnitId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Articles");
        }
    }
}
