using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRoleAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 2, null, "Admin", "ADMIN" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 2, 658, DateTimeKind.Utc).AddTicks(8748), "AQAAAAIAAYagAAAAEH3nbGYQBm88ITMrEDbJk/Ch0vs82AvyFNqehidj5mvyZaaVTGtercq6f9kOdqdxpQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 2, 829, DateTimeKind.Utc).AddTicks(9739), "AQAAAAIAAYagAAAAENXo6TjlPK8CJFxhsvPPalftMKStrLaI8kBrO/vHo6Y2q56eof5THJeCRq9ew/DdtQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 3, 2, DateTimeKind.Utc).AddTicks(2612), "AQAAAAIAAYagAAAAEOynP7KdjWQad1lVkSoYUWA5NrHhaO3H9stas8Wi7nEcrbQJZue7vZ41BImC0sHueA==", "User" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 3, 170, DateTimeKind.Utc).AddTicks(2588), "AQAAAAIAAYagAAAAEIo90/BHwgUFhkJt8Rq89YkuSSylVXr2MeFWGJC0RMg/nHkel7+1qn+ng7gO1CKvmw==" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "Password", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 5, 0, "f1e2d3c4-7777-8888-9999-aaaabbbbcccc", new DateTime(2026, 6, 27, 3, 4, 3, 340, DateTimeKind.Utc).AddTicks(2596), "admin@khedmetak.com", true, false, null, "Admin", "ADMIN@KHEDMETAK.COM", "ADMIN", "Admin@123", "AQAAAAIAAYagAAAAEIsPHDnXQqf0lVSiU4GofMTdR9dfGug2DoP+240DKaLM2KOHz4Cx/DgH518xG2y3GQ==", null, false, "Admin", "a1b2c3d4-1111-2222-3333-444455556666", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 2, 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 2, 42, 25, 161, DateTimeKind.Utc).AddTicks(406), "AQAAAAIAAYagAAAAEBmpF8S50KunsOHm/2WEdZW5R3EwGLGYMpnQUTVcli8UWttJI7RYW1VzIwfozAoDXQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 2, 42, 25, 379, DateTimeKind.Utc).AddTicks(5836), "AQAAAAIAAYagAAAAEGIiWMEYRiFDTgv03FgA4sLspaMmJTcUfB4JZh0bzYKypoIbqphEP8RotM0qd1to3Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2026, 6, 27, 2, 42, 25, 552, DateTimeKind.Utc).AddTicks(4163), "AQAAAAIAAYagAAAAEE4pBb5Dkg6Abaak0XcTAsreYYypECGc0MUE8EeRNZjl9HOw7Mhv9ho5O2/zPFDz+w==", "Admin" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 2, 42, 25, 724, DateTimeKind.Utc).AddTicks(1226), "AQAAAAIAAYagAAAAEDO4hqiFmvoHSdN2U3AibNwWTErsmA/lxQTDuvlMXsOr1IerbSmuV7Z+7r6FPcf9tQ==" });
        }
    }
}
