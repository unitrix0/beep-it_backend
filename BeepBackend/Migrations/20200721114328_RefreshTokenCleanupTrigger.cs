using BeepBackend.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using Utrix.WebLib.Helpers;

namespace BeepBackend.Migrations
{
    public partial class RefreshTokenCleanupTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlFromResource(typeof(BeepDbContext).Assembly,"BeepBackend.SQL Scripts.RefreshTokenCleanupTrigger.sql");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER [dbo].[CleanupAfterInsert]");
        }
    }
}
