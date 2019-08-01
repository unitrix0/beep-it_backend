using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddPermissionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsOwner = table.Column<bool>(nullable: false),
                    CanView = table.Column<bool>(nullable: false),
                    CheckIn = table.Column<bool>(nullable: false),
                    CheckOut = table.Column<bool>(nullable: false),
                    EditArticleSettings = table.Column<bool>(nullable: false),
                    Invite = table.Column<bool>(nullable: false),
                    RemoveMember = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
