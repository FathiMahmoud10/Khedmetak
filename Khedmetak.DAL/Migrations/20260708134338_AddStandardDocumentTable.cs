using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStandardDocumentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StandardDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneralRule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredDocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardDocuments_RequiredDocuments_RequiredDocumentId",
                        column: x => x.RequiredDocumentId,
                        principalTable: "RequiredDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 13, 43, 35, 924, DateTimeKind.Utc).AddTicks(4859), "AQAAAAIAAYagAAAAELufBgVJRf8hmm9DX0KHyGup1n6EMrMdZdgQzfVrGSJNQ6RQBVsEOdq5msvnvJlshA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 13, 43, 36, 14, DateTimeKind.Utc).AddTicks(2621), "AQAAAAIAAYagAAAAENAf1J3wml8lRLWEZbsKQT7qAW83V0QXpv+y5Q1vZ+V4UNYEqmwJr96BgtQzXM2uPA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 13, 43, 36, 109, DateTimeKind.Utc).AddTicks(2384), "AQAAAAIAAYagAAAAEADVuTLqHRCV/n61s+qiLROquSCLjKhIu8/Qq40aampNN5EZiCBkMRYKZ13EQpRBKw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 13, 43, 36, 205, DateTimeKind.Utc).AddTicks(4263), "AQAAAAIAAYagAAAAELkR6VWoiidn/OUnxaRzw4jYcIzzppf9dMGcVJW8H+0zb++QMJHHlDtRNdVw0H3FSQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 13, 43, 36, 291, DateTimeKind.Utc).AddTicks(2585), "AQAAAAIAAYagAAAAEFlR+7hxND+ku4VqTFVayjvMoE7dNO+J6Iui63wvC8A3XTuM2P0Qkx6vk7lGaq1gIA==" });

            migrationBuilder.CreateIndex(
                name: "IX_StandardDocuments_RequiredDocumentId",
                table: "StandardDocuments",
                column: "RequiredDocumentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardDocuments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 7, 22, 56, 10, 783, DateTimeKind.Utc).AddTicks(7100), "AQAAAAIAAYagAAAAECL1XsEh5h5JS5AItGhwEJfaqJAv7hDZQzaq5TIm01KtjE3Im4jxbLpHoEmlHAGsxQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 7, 22, 56, 10, 966, DateTimeKind.Utc).AddTicks(1576), "AQAAAAIAAYagAAAAEOMmbJpYU6WchABIlJ/Ata2esXJa6hNGZcgXwUJkjv7Fg4Yi0gMn/5yosyn9wB1Eiw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 7, 22, 56, 11, 136, DateTimeKind.Utc).AddTicks(6429), "AQAAAAIAAYagAAAAEOR22rgut5rkatFomxhu2SmURWeom8Q4CQd+49ulvmdyq0L4vR36QOsZjlAqCgNtBw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 7, 22, 56, 11, 305, DateTimeKind.Utc).AddTicks(4578), "AQAAAAIAAYagAAAAELgnkZbW0vdtYkTN5/TvYJfPI/YHzxUoOAw6l7jLeok6UK46tZpjuBOIHDkgwwUm8g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 7, 22, 56, 11, 485, DateTimeKind.Utc).AddTicks(8074), "AQAAAAIAAYagAAAAEI/mdLASZiES1oXiixkzFhlXSkewJLa0DlIrOReIrvJMiSlTeAE5ebPUTjINGqHWWw==" });
        }
    }
}
