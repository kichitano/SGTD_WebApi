using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_DocumentaryProcedureStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentaryProcedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcedures_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentaryProcedureSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentaryProcedureId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcedureSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcedureSteps_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcedureSteps_DocumentaryProcedures_DocumentaryProcedureId",
                        column: x => x.DocumentaryProcedureId,
                        principalTable: "DocumentaryProcedures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcedureSteps_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcedures_AreaId",
                table: "DocumentaryProcedures",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcedureSteps_AreaId",
                table: "DocumentaryProcedureSteps",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcedureSteps_DocumentaryProcedureId",
                table: "DocumentaryProcedureSteps",
                column: "DocumentaryProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcedureSteps_PositionId",
                table: "DocumentaryProcedureSteps",
                column: "PositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentaryProcedureSteps");

            migrationBuilder.DropTable(
                name: "DocumentaryProcedures");
        }
    }
}
