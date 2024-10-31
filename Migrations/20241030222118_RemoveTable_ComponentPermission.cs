using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTable_ComponentPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_ComponentPermissions_ComponentPermissionId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropTable(
                name: "ComponentPermissions");

            migrationBuilder.RenameColumn(
                name: "ComponentPermissionId",
                table: "RoleComponentPermissions",
                newName: "PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleComponentPermissions_ComponentPermissionId",
                table: "RoleComponentPermissions",
                newName: "IX_RoleComponentPermissions_PermissionId");

            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "RoleComponentPermissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoleComponentPermissions_ComponentId",
                table: "RoleComponentPermissions",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Components_ComponentId",
                table: "RoleComponentPermissions",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Permissions_PermissionId",
                table: "RoleComponentPermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Components_ComponentId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Permissions_PermissionId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropIndex(
                name: "IX_RoleComponentPermissions_ComponentId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "RoleComponentPermissions");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "RoleComponentPermissions",
                newName: "ComponentPermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleComponentPermissions_PermissionId",
                table: "RoleComponentPermissions",
                newName: "IX_RoleComponentPermissions_ComponentPermissionId");

            migrationBuilder.CreateTable(
                name: "ComponentPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentPermissions_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComponentPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentPermissions_ComponentId",
                table: "ComponentPermissions",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentPermissions_PermissionId",
                table: "ComponentPermissions",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_ComponentPermissions_ComponentPermissionId",
                table: "RoleComponentPermissions",
                column: "ComponentPermissionId",
                principalTable: "ComponentPermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
