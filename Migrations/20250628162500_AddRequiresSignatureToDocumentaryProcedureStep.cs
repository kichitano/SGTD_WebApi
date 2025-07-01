using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    public partial class AddRequiresSignatureToDocumentaryProcedureStep : Migration
    {
            protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresSignature",
                table: "DocumentaryProcedureSteps",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

            protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresSignature",
                table: "DocumentaryProcedureSteps");
        }
    }
}