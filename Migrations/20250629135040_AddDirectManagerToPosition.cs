using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    public partial class AddDirectManagerToPosition : Migration
    {
            protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DirectManagerPositionId",
                table: "Positions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DirectManagerPositionId",
                table: "Positions",
                column: "DirectManagerPositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Positions_DirectManagerPositionId",
                table: "Positions",
                column: "DirectManagerPositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }

            protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Positions_DirectManagerPositionId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_DirectManagerPositionId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "DirectManagerPositionId",
                table: "Positions");
        }
    }
}
