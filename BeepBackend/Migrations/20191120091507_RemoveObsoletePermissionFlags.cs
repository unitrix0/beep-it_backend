using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RemoveObsoletePermissionFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanView",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CheckIn",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CheckOut",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Invite",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RemoveMember",
                table: "Permissions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanView",
                table: "Permissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CheckIn",
                table: "Permissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CheckOut",
                table: "Permissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Invite",
                table: "Permissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RemoveMember",
                table: "Permissions",
                nullable: false,
                defaultValue: false);
        }
    }
}
