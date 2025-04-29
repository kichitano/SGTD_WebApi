using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable_Authenticator_Authenticators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Authenticator",
                table: "Authenticator");

            migrationBuilder.RenameTable(
                name: "Authenticator",
                newName: "Authenticators");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authenticators",
                table: "Authenticators",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Authenticators",
                table: "Authenticators");

            migrationBuilder.RenameTable(
                name: "Authenticators",
                newName: "Authenticator");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authenticator",
                table: "Authenticator",
                column: "Id");
        }
    }
}
