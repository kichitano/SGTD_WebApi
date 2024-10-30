using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Components_ComponentId",
                table: "RoleComponentPermissions",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Permissions_PermissionId",
                table: "RoleComponentPermissions",
                column: "PermissionId",
                principalTable: "Permissions",
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
    }
}
