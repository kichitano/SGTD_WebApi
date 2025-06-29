using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLogicalDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserTokens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserRoles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserRoles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserFileShares",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserFileShares",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserFiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserDigitalSignatures",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserDigitalSignatures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RoleComponentPermissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RoleComponentPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Positions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Positions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PositionDependencies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PositionDependencies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Persons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DocumentTypes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DocumentTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DocumentaryProcedureSteps",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DocumentaryProcedureSteps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DocumentaryProcedureStepDocuments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DocumentaryProcedureStepDocuments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DocumentaryProcedures",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DocumentaryProcedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Components",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Components",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Authenticators",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Authenticators",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Areas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Areas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AreaDependencies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AreaDependencies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserFileShares");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserFileShares");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserDigitalSignatures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserDigitalSignatures");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RoleComponentPermissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RoleComponentPermissions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PositionDependencies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PositionDependencies");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DocumentaryProcedureSteps");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DocumentaryProcedureSteps");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DocumentaryProcedureStepDocuments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DocumentaryProcedureStepDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DocumentaryProcedures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DocumentaryProcedures");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Authenticators");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Authenticators");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AreaDependencies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AreaDependencies");
        }
    }
}
