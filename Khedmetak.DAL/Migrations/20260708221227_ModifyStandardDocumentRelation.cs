using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStandardDocumentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StandardDocuments_RequiredDocuments_RequiredDocumentId",
                table: "StandardDocuments");

            migrationBuilder.DropIndex(
                name: "IX_StandardDocuments_RequiredDocumentId",
                table: "StandardDocuments");

            migrationBuilder.DropColumn(
                name: "RequiredDocumentId",
                table: "StandardDocuments");

            migrationBuilder.AddColumn<int>(
                name: "StandardDocumentId",
                table: "RequiredDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 580, DateTimeKind.Utc).AddTicks(6376), "AQAAAAIAAYagAAAAECs4+e5Hpbc1XIZ2OjaU/sl3fq8A9qEAj2gbmbCHTspUhBMq4jUiOYtB9me0k5HcmA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 689, DateTimeKind.Utc).AddTicks(6988), "AQAAAAIAAYagAAAAEO2U4hjjV9DWCz0UB2wRB/z/BHkWxnunJaxxRdWKJTlE1Ix4jSR/kXsPplWlGt6yKg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 786, DateTimeKind.Utc).AddTicks(6551), "AQAAAAIAAYagAAAAEM5dRbFge1fpdcVaPl+SxaZ0D2/q3v1aBB9kwxPcCz6vPEJUzqM4eSVXJLuH5wPUYw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 878, DateTimeKind.Utc).AddTicks(8918), "AQAAAAIAAYagAAAAEGjNyTMOpPH9tqpmUi6knJseo9f1BS5BbPLVUASeLFcj0rQIf7yexZgKg1T+zUQeYQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 986, DateTimeKind.Utc).AddTicks(6402), "AQAAAAIAAYagAAAAELzLNLFffOmd5Z0QBDQbwWMWhoNsi2o7Ni/OfuETX6c5p9h7L31KUWgEBqf+NHetSg==" });

            migrationBuilder.CreateIndex(
                name: "IX_RequiredDocuments_StandardDocumentId",
                table: "RequiredDocuments",
                column: "StandardDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequiredDocuments_StandardDocuments_StandardDocumentId",
                table: "RequiredDocuments",
                column: "StandardDocumentId",
                principalTable: "StandardDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequiredDocuments_StandardDocuments_StandardDocumentId",
                table: "RequiredDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RequiredDocuments_StandardDocumentId",
                table: "RequiredDocuments");

            migrationBuilder.DropColumn(
                name: "StandardDocumentId",
                table: "RequiredDocuments");

            migrationBuilder.AddColumn<int>(
                name: "RequiredDocumentId",
                table: "StandardDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_StandardDocuments_RequiredDocuments_RequiredDocumentId",
                table: "StandardDocuments",
                column: "RequiredDocumentId",
                principalTable: "RequiredDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
