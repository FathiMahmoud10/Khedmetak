using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FawryEntityPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantRefNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FawryRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 17, 33, 51, 559, DateTimeKind.Utc).AddTicks(1095), "AQAAAAIAAYagAAAAEA44m5ML1cLAifR4E/BL0RFvxmt0MmaU3nhNYUT7I/6ZzCJrt/XZr6XhS2TKsa+trQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 17, 33, 51, 656, DateTimeKind.Utc).AddTicks(7194), "AQAAAAIAAYagAAAAEEzpEfxE0QVBYjEkxFoo0oa1eEw30LfZcjfWiMSfZbwI6oRiuHeeWIaqqEFG9b7vQw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 17, 33, 51, 758, DateTimeKind.Utc).AddTicks(9721), "AQAAAAIAAYagAAAAEKwI5fCDA4mfCTstAI753JPe4s+Tlb5tHzgztvJmL1+x6mf4j5a24vM1/lsixkFNvw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 17, 33, 51, 855, DateTimeKind.Utc).AddTicks(1646), "AQAAAAIAAYagAAAAENe3nn9Y9YYtdWejx6FCn/udtyANOIh+3699KfSRkpq/TQ6TmH67vJ2MQMcN+DWhRA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 29, 17, 33, 51, 959, DateTimeKind.Utc).AddTicks(4330), "AQAAAAIAAYagAAAAECdoumrn4Du4SFj16KLK6FL75TqGjj9gVkOOjUQ23wDEqqGN6+7FVSmbEk1erOtI1Q==" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

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
        }
    }
}
