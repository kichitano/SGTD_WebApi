using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_DocumentaryProcedureStepDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentaryProcedureStepDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentaryProcedureStepId = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcedureStepDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcedureStepDocuments_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcedureStepDocuments_DocumentaryProcedureSteps_DocumentaryProcedureStepId",
                        column: x => x.DocumentaryProcedureStepId,
                        principalTable: "DocumentaryProcedureSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcedureStepDocuments_DocumentaryProcedureStepId",
                table: "DocumentaryProcedureStepDocuments",
                column: "DocumentaryProcedureStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcedureStepDocuments_DocumentTypeId",
                table: "DocumentaryProcedureStepDocuments",
                column: "DocumentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentaryProcedureStepDocuments");
        }
    }
}
