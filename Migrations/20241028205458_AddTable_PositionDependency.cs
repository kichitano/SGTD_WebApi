using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_PositionDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hierarchy",
                table: "Positions");

            migrationBuilder.CreateTable(
                name: "PositionDependencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentPositionId = table.Column<int>(type: "int", nullable: false),
                    ChildPositionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionDependencies_Positions_ChildPositionId",
                        column: x => x.ChildPositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PositionDependencies_Positions_ParentPositionId",
                        column: x => x.ParentPositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionDependencies_ChildPositionId",
                table: "PositionDependencies",
                column: "ChildPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionDependencies_ParentPositionId",
                table: "PositionDependencies",
                column: "ParentPositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionDependencies");

            migrationBuilder.AddColumn<int>(
                name: "Hierarchy",
                table: "Positions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
