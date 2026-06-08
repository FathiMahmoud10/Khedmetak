using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_AspNetUsers_UserId",
                table: "ChatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_GovernmentServices_GovernmentServiceId",
                table: "ChatSessions");

            migrationBuilder.DropIndex(
                name: "IX_ChatSessions_UserId",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "ChatSessions");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "ChatSessions",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "PreferredLanguage",
                table: "AspNetUsers",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "AspNetUsers",
                newName: "Password");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ChatSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "GovernmentServiceId",
                table: "ChatSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "ChatSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "ChatSessions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChatSessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_ChatSessions_UserId1",
                table: "ChatSessions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryChatSession_ChatSessionsId",
                table: "CategoryChatSession",
                column: "ChatSessionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatSessionId",
                table: "ChatMessage",
                column: "ChatSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_AspNetUsers_UserId1",
                table: "ChatSessions",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_GovernmentServices_GovernmentServiceId",
                table: "ChatSessions",
                column: "GovernmentServiceId",
                principalTable: "GovernmentServices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_AspNetUsers_UserId1",
                table: "ChatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_GovernmentServices_GovernmentServiceId",
                table: "ChatSessions");

            migrationBuilder.DropTable(
                name: "CategoryChatSession");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_ChatSessions_UserId1",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "ChatSessions",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "AspNetUsers",
                newName: "PreferredLanguage");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "AspNetUsers",
                newName: "FullName");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ChatSessions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GovernmentServiceId",
                table: "ChatSessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ChatSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "ChatSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_UserId",
                table: "ChatSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_AspNetUsers_UserId",
                table: "ChatSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_GovernmentServices_GovernmentServiceId",
                table: "ChatSessions",
                column: "GovernmentServiceId",
                principalTable: "GovernmentServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
