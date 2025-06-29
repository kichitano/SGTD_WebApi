using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGTD_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentaryProcessEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentaryProcessInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentaryProcedureId = table.Column<int>(type: "integer", nullable: false),
                    RequestedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStepOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcessInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessInstances_DocumentaryProcedures_Documenta~",
                        column: x => x.DocumentaryProcedureId,
                        principalTable: "DocumentaryProcedures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessInstances_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentaryProcessNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentaryProcessInstanceId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcessNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessNotifications_DocumentaryProcessInstances~",
                        column: x => x.DocumentaryProcessInstanceId,
                        principalTable: "DocumentaryProcessInstances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentaryProcessStepInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentaryProcessInstanceId = table.Column<int>(type: "integer", nullable: false),
                    DocumentaryProcedureStepId = table.Column<int>(type: "integer", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcessStepInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessStepInstances_DocumentaryProcedureSteps_D~",
                        column: x => x.DocumentaryProcedureStepId,
                        principalTable: "DocumentaryProcedureSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessStepInstances_DocumentaryProcessInstances~",
                        column: x => x.DocumentaryProcessInstanceId,
                        principalTable: "DocumentaryProcessInstances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessStepInstances_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentaryProcessDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentaryProcessInstanceId = table.Column<int>(type: "integer", nullable: false),
                    DocumentaryProcessStepInstanceId = table.Column<int>(type: "integer", nullable: true),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: false),
                    UploadedByUserId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSignatureRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsSigned = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentaryProcessDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessDocuments_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessDocuments_DocumentaryProcessInstances_Doc~",
                        column: x => x.DocumentaryProcessInstanceId,
                        principalTable: "DocumentaryProcessInstances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessDocuments_DocumentaryProcessStepInstances~",
                        column: x => x.DocumentaryProcessStepInstanceId,
                        principalTable: "DocumentaryProcessStepInstances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentaryProcessDocuments_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessDocuments_DocumentaryProcessInstanceId",
                table: "DocumentaryProcessDocuments",
                column: "DocumentaryProcessInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessDocuments_DocumentaryProcessStepInstanceId",
                table: "DocumentaryProcessDocuments",
                column: "DocumentaryProcessStepInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessDocuments_DocumentTypeId",
                table: "DocumentaryProcessDocuments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessDocuments_UploadedByUserId",
                table: "DocumentaryProcessDocuments",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessInstances_DocumentaryProcedureId",
                table: "DocumentaryProcessInstances",
                column: "DocumentaryProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessInstances_RequestedByUserId",
                table: "DocumentaryProcessInstances",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessNotifications_DocumentaryProcessInstanceId",
                table: "DocumentaryProcessNotifications",
                column: "DocumentaryProcessInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessNotifications_UserId",
                table: "DocumentaryProcessNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessStepInstances_AssignedToUserId",
                table: "DocumentaryProcessStepInstances",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessStepInstances_DocumentaryProcedureStepId",
                table: "DocumentaryProcessStepInstances",
                column: "DocumentaryProcedureStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentaryProcessStepInstances_DocumentaryProcessInstanceId",
                table: "DocumentaryProcessStepInstances",
                column: "DocumentaryProcessInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentaryProcessDocuments");

            migrationBuilder.DropTable(
                name: "DocumentaryProcessNotifications");

            migrationBuilder.DropTable(
                name: "DocumentaryProcessStepInstances");

            migrationBuilder.DropTable(
                name: "DocumentaryProcessInstances");
        }
    }
}
