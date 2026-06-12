using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserDocuments",
                newName: "FileType");

            migrationBuilder.AlterColumn<int>(
                name: "ChatSessionId",
                table: "UserDocuments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "UserDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "UserDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 1, null, "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "Password", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, "e7492cfa-e160-49b8-a6d1-817abcf992bf", new DateTime(2026, 6, 12, 6, 55, 37, 688, DateTimeKind.Utc).AddTicks(8380), "fathi@khedmetak.com", true, false, null, "Fathi", "FATHI@KHEDMETAK.COM", "FATHI", "12345678", "AQAAAAIAAYagAAAAEMjg81hSF5F3ZxN72nU4PBTrf7p3bBpfDvpDl9lA1NdvkgznTpUnC5iTzCgUU2JJ4Q==", null, false, "User", "f4fb76b8-2ea9-42b7-876a-39fbcf9e6cf4", false, "Fathi" },
                    { 2, 0, "df768913-9118-4a9f-a496-e26bbbc23eef", new DateTime(2026, 6, 12, 6, 55, 37, 789, DateTimeKind.Utc).AddTicks(386), "aya@khedmetak.com", true, false, null, "Aya", "AYA@KHEDMETAK.COM", "AYA", "12345678", "AQAAAAIAAYagAAAAEDfhWTn75o4fP33Mq3RkVs/R/iN08IBTi9eVI6huDktohBnkZl+OVPl/OjOrB6l5Fg==", null, false, "User", "bc521d96-c167-4277-a859-00ef1295beea", false, "Aya" },
                    { 3, 0, "b1f5fe6b-67a4-44b7-bdc6-2c93d9fb34d0", new DateTime(2026, 6, 12, 6, 55, 37, 897, DateTimeKind.Utc).AddTicks(6855), "naglaa@khedmetak.com", true, false, null, "Naglaa", "NAGLAA@KHEDMETAK.COM", "NAGLAA", "12345678", "AQAAAAIAAYagAAAAEGxlLh2C/BTcrSescFMI2b5ouIq4Sm6Zins9kHzVWJbl2vLXfM2nXuvFPGXXJFBPpQ==", null, false, "User", "cbe62da6-dbdb-4fbc-bdf8-18e388ffc811", false, "Naglaa" },
                    { 4, 0, "5c5fbef1-cb69-42b7-99e2-348f6cfef7e9", new DateTime(2026, 6, 12, 6, 55, 37, 998, DateTimeKind.Utc).AddTicks(5988), "rahma@khedmetak.com", true, false, null, "Rahma", "RAHMA@KHEDMETAK.COM", "RAHMA", "12345678", "AQAAAAIAAYagAAAAEIUshO+mMrcjPFxnUXe6yJYTBOBMrOs8Ja8SmaFrKRoPRbhkUbjgS3Jlnh6DlXQJbA==", null, false, "User", "d7d91e6b-e53b-4861-a53d-82c5f1fa6d03", false, "Rahma" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "UserDocuments");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "UserDocuments");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "UserDocuments",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "ChatSessionId",
                table: "UserDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
