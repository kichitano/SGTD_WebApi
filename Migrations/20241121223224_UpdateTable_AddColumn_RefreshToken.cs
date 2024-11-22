using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable_AddColumn_RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserTokens");
        }
    }
}
