using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class MoveRequiresSignatureToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar RequiresSignature de DocumentaryProcedureSteps
            migrationBuilder.DropColumn(
                name: "RequiresSignature",
                table: "DocumentaryProcedureSteps");

            // Agregar RequiresSignature a DocumentaryProcedureStepDocuments
            migrationBuilder.AddColumn<bool>(
                name: "RequiresSignature",
                table: "DocumentaryProcedureStepDocuments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar RequiresSignature de DocumentaryProcedureStepDocuments
            migrationBuilder.DropColumn(
                name: "RequiresSignature",
                table: "DocumentaryProcedureStepDocuments");

            // Agregar RequiresSignature a DocumentaryProcedureSteps
            migrationBuilder.AddColumn<bool>(
                name: "RequiresSignature",
                table: "DocumentaryProcedureSteps",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}