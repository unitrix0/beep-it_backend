using System.Diagnostics;
using BeepBackend.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;
using Utrix.WebLib.Helpers;

namespace BeepBackend.Migrations
{
    public partial class UpdateCreateDemoData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlFromResource("BeepBackend.SQL Scripts.CreateDemoDataForUser.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[CreateDemoDataForUser]");
        }
    }
}
