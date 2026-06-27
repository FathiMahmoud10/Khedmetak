using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditseedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 5, 58, 292, DateTimeKind.Utc).AddTicks(427), "AQAAAAIAAYagAAAAEMunpPU4BX4nS+HPNrqrv1F/Ft3H2Mvl1MYnLjqYhYU/2uBWifqTo9TyGGi+f7sfyg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 5, 58, 489, DateTimeKind.Utc).AddTicks(3107), "AQAAAAIAAYagAAAAED9MIhqwCMwRSTwgeDM+U6tWDeLLNUHiNamsoS8NRU6tinbnAcFeomKKWp760uUh0A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 5, 58, 658, DateTimeKind.Utc).AddTicks(300), "AQAAAAIAAYagAAAAEGPLnyUCW1/6733YLQ2Wk5p79nG/6/L0L1NCztEC4nAF2dz3z+7Q0Yw9NUYmVO6FoQ==", "User" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 27, 0, 5, 58, 824, DateTimeKind.Utc).AddTicks(6350), "AQAAAAIAAYagAAAAENPShhh3ZTiHvUGboaQg2y4RyHtkidjhFOpBofyvUGe4NeTYwn3iMMXxxHCVd7H8bw==" });
        }
    }
}
