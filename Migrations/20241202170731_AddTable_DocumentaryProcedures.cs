using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTable_DocumentaryProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaDependencies_Areas_ChildAreaId",
                table: "AreaDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_AreaDependencies_Areas_ParentAreaId",
                table: "AreaDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionDependencies_Positions_ChildPositionId",
                table: "PositionDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionDependencies_Positions_ParentPositionId",
                table: "PositionDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Components_ComponentId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Permissions_PermissionId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFiles_Users_UserId",
                table: "UserFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPositions_Positions_PositionId",
                table: "UserPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPositions_Users_UserId",
                table: "UserPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_People_PersonId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaDependencies_Areas_ChildAreaId",
                table: "AreaDependencies",
                column: "ChildAreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaDependencies_Areas_ParentAreaId",
                table: "AreaDependencies",
                column: "ParentAreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionDependencies_Positions_ChildPositionId",
                table: "PositionDependencies",
                column: "ChildPositionId",
                principalTable: "Positions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionDependencies_Positions_ParentPositionId",
                table: "PositionDependencies",
                column: "ParentPositionId",
                principalTable: "Positions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Components_ComponentId",
                table: "RoleComponentPermissions",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Permissions_PermissionId",
                table: "RoleComponentPermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFiles_Users_UserId",
                table: "UserFiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPositions_Positions_PositionId",
                table: "UserPositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPositions_Users_UserId",
                table: "UserPositions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_People_PersonId",
                table: "Users",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaDependencies_Areas_ChildAreaId",
                table: "AreaDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_AreaDependencies_Areas_ParentAreaId",
                table: "AreaDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionDependencies_Positions_ChildPositionId",
                table: "PositionDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionDependencies_Positions_ParentPositionId",
                table: "PositionDependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Components_ComponentId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Permissions_PermissionId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleComponentPermissions_Roles_RoleId",
                table: "RoleComponentPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFiles_Users_UserId",
                table: "UserFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPositions_Positions_PositionId",
                table: "UserPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPositions_Users_UserId",
                table: "UserPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_People_PersonId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaDependencies_Areas_ChildAreaId",
                table: "AreaDependencies",
                column: "ChildAreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AreaDependencies_Areas_ParentAreaId",
                table: "AreaDependencies",
                column: "ParentAreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionDependencies_Positions_ChildPositionId",
                table: "PositionDependencies",
                column: "ChildPositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionDependencies_Positions_ParentPositionId",
                table: "PositionDependencies",
                column: "ParentPositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserFiles_Users_UserId",
                table: "UserFiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPositions_Positions_PositionId",
                table: "UserPositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPositions_Users_UserId",
                table: "UserPositions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_People_PersonId",
                table: "Users",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
