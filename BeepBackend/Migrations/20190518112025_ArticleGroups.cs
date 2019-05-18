using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class ArticleGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleGroupFk",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ArticleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleGroupFk",
                table: "Articles",
                column: "ArticleGroupFk");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupFk",
                table: "Articles",
                column: "ArticleGroupFk",
                principalTable: "ArticleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupFk",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "ArticleGroups");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleGroupFk",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleGroupFk",
                table: "Articles");
        }
    }
}
