using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityClasses2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_ChatSessions_ChatSessionId",
                table: "ChatMessage");

            migrationBuilder.DropTable(
                name: "CategoryChatSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "ChatMessage",
                newName: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessage_ChatSessionId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_ChatSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChatSessionCategory",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    ChatSessionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessionCategory", x => new { x.CategoriesId, x.ChatSessionsId });
                    table.ForeignKey(
                        name: "FK_ChatSessionCategory_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatSessionCategory_ChatSessions_ChatSessionsId",
                        column: x => x.ChatSessionsId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedBackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChatSessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedBackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GovServices",
                columns: table => new
                {
                    SrvId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SrvName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SrvDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SrvFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SrvTime = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovServices", x => x.SrvId);
                    table.ForeignKey(
                        name: "FK_GovServices_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeBases_GovServices_GovServiceId",
                        column: x => x.GovServiceId,
                        principalTable: "GovServices",
                        principalColumn: "SrvId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequiredDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredDocuments", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_RequiredDocuments_GovServices_GovServiceId",
                        column: x => x.GovServiceId,
                        principalTable: "GovServices",
                        principalColumn: "SrvId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceSteps_GovServices_GovServiceId",
                        column: x => x.GovServiceId,
                        principalTable: "GovServices",
                        principalColumn: "SrvId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredDocumentDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDocuments_RequiredDocuments_RequiredDocumentDocumentId",
                        column: x => x.RequiredDocumentDocumentId,
                        principalTable: "RequiredDocuments",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessionCategory_ChatSessionsId",
                table: "ChatSessionCategory",
                column: "ChatSessionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ChatSessionId",
                table: "Feedbacks",
                column: "ChatSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GovServices_CategoryId",
                table: "GovServices",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBases_GovServiceId",
                table: "KnowledgeBases",
                column: "GovServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RequiredDocuments_GovServiceId",
                table: "RequiredDocuments",
                column: "GovServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSteps_GovServiceId",
                table: "ServiceSteps",
                column: "GovServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_RequiredDocumentDocumentId",
                table: "UserDocuments",
                column: "RequiredDocumentDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                table: "ChatMessages",
                column: "ChatSessionId",
                principalTable: "ChatSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatSessionCategory");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "KnowledgeBases");

            migrationBuilder.DropTable(
                name: "ServiceSteps");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.DropTable(
                name: "RequiredDocuments");

            migrationBuilder.DropTable(
                name: "GovServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "ChatMessage");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_ChatSessionId",
                table: "ChatMessage",
                newName: "IX_ChatMessage_ChatSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoryChatSession",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    ChatSessionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryChatSession", x => new { x.CategoriesId, x.ChatSessionsId });
                    table.ForeignKey(
                        name: "FK_CategoryChatSession_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryChatSession_ChatSessions_ChatSessionsId",
                        column: x => x.ChatSessionsId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryChatSession_ChatSessionsId",
                table: "CategoryChatSession",
                column: "ChatSessionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_ChatSessions_ChatSessionId",
                table: "ChatMessage",
                column: "ChatSessionId",
                principalTable: "ChatSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
