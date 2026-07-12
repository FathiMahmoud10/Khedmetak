using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddChatLimitsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatMessagesCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasPaidForChat",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ChatMessagesCount", "CreatedAt", "HasPaidForChat", "PasswordHash" },
                values: new object[] { 0, new DateTime(2026, 7, 11, 23, 38, 12, 465, DateTimeKind.Utc).AddTicks(3980), false, "AQAAAAIAAYagAAAAEO1gOXTx78OpErnSHgnRf1bisfPK/lpXdr24GnIx7zDosOH1cYj1YOhR8tuvklqv0Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ChatMessagesCount", "CreatedAt", "HasPaidForChat", "PasswordHash" },
                values: new object[] { 0, new DateTime(2026, 7, 11, 23, 38, 12, 584, DateTimeKind.Utc).AddTicks(6834), false, "AQAAAAIAAYagAAAAEOYJPSMNTDOOV3lRRUdccb+Y3dIntrrEkxl6snw4h116wSJt3tt14O6vYQ+Tgt7IGg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ChatMessagesCount", "CreatedAt", "HasPaidForChat", "PasswordHash" },
                values: new object[] { 0, new DateTime(2026, 7, 11, 23, 38, 12, 744, DateTimeKind.Utc).AddTicks(1311), false, "AQAAAAIAAYagAAAAELPfunkwwtkU9LQnbP3hPxIDO2njxQGl4M4GdLr4JA4V+JPm2fQIw8CMccmOlZF0OQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ChatMessagesCount", "CreatedAt", "HasPaidForChat", "PasswordHash" },
                values: new object[] { 0, new DateTime(2026, 7, 11, 23, 38, 12, 872, DateTimeKind.Utc).AddTicks(3584), false, "AQAAAAIAAYagAAAAEAERxvyv+zZNQBzPw/fUrIcnrgm053EwLyEi3o+Hd9XzHZbt21rdex5bBXM9XMzHhA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ChatMessagesCount", "CreatedAt", "HasPaidForChat", "PasswordHash" },
                values: new object[] { 0, new DateTime(2026, 7, 11, 23, 38, 13, 37, DateTimeKind.Utc).AddTicks(4615), false, "AQAAAAIAAYagAAAAEGPdpixrWj1+cC7F4lFg3uxEo7/2y7EM4vxTrM5oFx13+g4Zl0tkj6QCBX4u02066g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatMessagesCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HasPaidForChat",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 2, 25, 768, DateTimeKind.Utc).AddTicks(7938), "AQAAAAIAAYagAAAAENLXB+gMXiIy0O4vPNGedPk2OeMkjQ0z9SPeo2Q1mI+K4uDLH/9l8BkkvewPF2MqJA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 2, 25, 854, DateTimeKind.Utc).AddTicks(745), "AQAAAAIAAYagAAAAEFj8Qn2E5nfy74V+w+lGz9Qp0+IMjrQ/KSbkSu2bjgRoaDHCqy4GwShvFfjO1Z+7bw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 2, 25, 940, DateTimeKind.Utc).AddTicks(4642), "AQAAAAIAAYagAAAAEDkRu7izXS/1i2JTAPo+K2mgOOhc/E6Xpfa4laFU1dOnmHbGvzN/dH5G/6Z8v+rBNw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 2, 26, 25, DateTimeKind.Utc).AddTicks(9756), "AQAAAAIAAYagAAAAEGXyo0eyC5cBxCOJAhaHfLZL47AnL/xlwpCvUYDtisPMJ9eOM/+gT3WJRAl4vN1CxA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 2, 26, 116, DateTimeKind.Utc).AddTicks(9055), "AQAAAAIAAYagAAAAEDY1Rvm7HX9dSTSqyzKAh4Ctqm7inSBYMkYgG29J87P+b3luXDm2sYcjNoCl4MwR5w==" });
        }
    }
}
