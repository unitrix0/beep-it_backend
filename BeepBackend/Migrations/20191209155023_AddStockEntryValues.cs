using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddStockEntryValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountOnStock",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "AmountRemaining",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "IsOpened",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "OpenedOn",
                table: "StockEntries");

            migrationBuilder.CreateTable(
                name: "StockEntryValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsOpened = table.Column<bool>(nullable: false),
                    AmountOnStock = table.Column<int>(nullable: false),
                    OpenedOn = table.Column<DateTime>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    AmountRemaining = table.Column<float>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntryValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockEntryValues_StockEntries_EnvironmentId_ArticleId",
                        columns: x => new { x.EnvironmentId, x.ArticleId },
                        principalTable: "StockEntries",
                        principalColumns: new[] { "EnvironmentId", "ArticleId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockEntryValues_EnvironmentId_ArticleId",
                table: "StockEntryValues",
                columns: new[] { "EnvironmentId", "ArticleId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockEntryValues");

            migrationBuilder.AddColumn<int>(
                name: "AmountOnStock",
                table: "StockEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "AmountRemaining",
                table: "StockEntries",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "StockEntries",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsOpened",
                table: "StockEntries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedOn",
                table: "StockEntries",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
