using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RemoveUnitOnArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ArticleUnitId",
                table: "Articles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockEntryValues_ArticleId",
                table: "StockEntryValues",
                column: "ArticleId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_StockEntryValues_Articles_ArticleId",
                table: "StockEntryValues",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleUnits_ArticleUnitId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_StockEntryValues_Articles_ArticleId",
                table: "StockEntryValues");

            migrationBuilder.DropIndex(
                name: "IX_StockEntryValues_ArticleId",
                table: "StockEntryValues");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleUnitId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleUnitId",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

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
    }
}
