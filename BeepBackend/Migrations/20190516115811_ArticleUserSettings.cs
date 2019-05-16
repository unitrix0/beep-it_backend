using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class ArticleUserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleUserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockAmount = table.Column<int>(nullable: false),
                    KeppStockMode = table.Column<int>(nullable: false),
                    IsOpened = table.Column<bool>(nullable: false),
                    OpenedOn = table.Column<DateTime>(nullable: false),
                    AmountOnStock = table.Column<int>(nullable: false),
                    ArticleFk = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleUserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleUserSettings_Articles_ArticleFk",
                        column: x => x.ArticleFk,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUserSettings_ArticleFk",
                table: "ArticleUserSettings",
                column: "ArticleFk");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleUserSettings");
        }
    }
}
