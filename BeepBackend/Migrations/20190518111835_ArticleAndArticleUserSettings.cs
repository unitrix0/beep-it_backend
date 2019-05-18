using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class ArticleAndArticleUserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    TypicalLifetime = table.Column<int>(nullable: false),
                    HasLifetime = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleUserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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

            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
