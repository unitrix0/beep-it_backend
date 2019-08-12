using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddRelationPermissionEnvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentId",
                table: "Permissions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_EnvironmentId",
                table: "Permissions",
                column: "EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Environments_EnvironmentId",
                table: "Permissions",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Environments_EnvironmentId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_EnvironmentId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "Permissions");
        }
    }
}
