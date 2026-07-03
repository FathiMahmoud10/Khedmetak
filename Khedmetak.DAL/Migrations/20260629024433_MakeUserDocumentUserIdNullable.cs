using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserDocumentUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserDocuments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 2, 44, 27, 263, DateTimeKind.Utc).AddTicks(5921), "AQAAAAIAAYagAAAAELgvGWQoULvFojh4tpM4qSSwSTZU4ugrZ7FAJ9NMrIMAl8+ft77SqHJ1GDmS+NRU8w==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 2, 44, 27, 439, DateTimeKind.Utc).AddTicks(6960), "AQAAAAIAAYagAAAAEITLkGBglOoEiH4T0hhiZPKW7I+Hm8lF9yFSQdLpRDANIVUCfjiOI8xvJ6qMmf2FfQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 2, 44, 27, 615, DateTimeKind.Utc).AddTicks(8736), "AQAAAAIAAYagAAAAEMfYT6pmtZYhn4jaQklRxux9f7N/1g2ZQRj2Y6kTcGblVBmF9HsrwE8O5HBk35uj5w==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 2, 44, 27, 791, DateTimeKind.Utc).AddTicks(7574), "AQAAAAIAAYagAAAAEN5r3hXDjPEgj31KUJKeWDuftGSL7z3DM8YEcjYtq3GJ858aBm+Xkww3rEob2+3AEw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 2, 44, 27, 966, DateTimeKind.Utc).AddTicks(3252), "AQAAAAIAAYagAAAAEGernec2bmzOCRYhdRCQkvdz9kqXhjD/T/OSkqO1o1u/xNaovPB1tEyfkvNAX3UCwQ==" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 3, 2, DateTimeKind.Utc).AddTicks(2612), "AQAAAAIAAYagAAAAEOynP7KdjWQad1lVkSoYUWA5NrHhaO3H9stas8Wi7nEcrbQJZue7vZ41BImC0sHueA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 3, 170, DateTimeKind.Utc).AddTicks(2588), "AQAAAAIAAYagAAAAEIo90/BHwgUFhkJt8Rq89YkuSSylVXr2MeFWGJC0RMg/nHkel7+1qn+ng7gO1CKvmw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 3, 4, 3, 340, DateTimeKind.Utc).AddTicks(2596), "AQAAAAIAAYagAAAAEIsPHDnXQqf0lVSiU4GofMTdR9dfGug2DoP+240DKaLM2KOHz4Cx/DgH518xG2y3GQ==" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_AspNetUsers_UserId",
                table: "UserDocuments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
