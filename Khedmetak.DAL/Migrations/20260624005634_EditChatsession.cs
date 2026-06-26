using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditChatsession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GovServiceId",
                table: "ChatSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChatSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CitizenProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitizenProfile_AspNetUsers_UserId",
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
                values: new object[] { new DateTime(2026, 6, 24, 0, 56, 29, 87, DateTimeKind.Utc).AddTicks(7166), "AQAAAAIAAYagAAAAEMGXFfLrmYyqR9fw2ooiY1BFblhRz1jpkvLzWLD9yzwkvlPFBQ/6/qEqIgxjkSGrvw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 24, 0, 56, 29, 263, DateTimeKind.Utc).AddTicks(9005), "AQAAAAIAAYagAAAAEBbJQonfZtum0+Oy+6Z+TtclrEetxTjNSouYm2gCzvXArT6UL3R1wOpvFOQ6YtzR5w==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 24, 0, 56, 29, 438, DateTimeKind.Utc).AddTicks(4565), "AQAAAAIAAYagAAAAEHEcByy8pLtzMDL9nCLWGL/gBm9XGf5pWtMn5muAA0eIVb4NmUvTKFmNATzaB0o3ew==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 24, 0, 56, 29, 613, DateTimeKind.Utc).AddTicks(2553), "AQAAAAIAAYagAAAAEHIQsVAGfe6fuqTx/+ty+yiqlFq/nk54wV/abpp7/AWRSoL+vr88DpWaoMtUEO+aiA==" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_GovServiceId",
                table: "ChatSessions",
                column: "GovServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenProfile_UserId",
                table: "CitizenProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_GovServices_GovServiceId",
                table: "ChatSessions",
                column: "GovServiceId",
                principalTable: "GovServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_GovServices_GovServiceId",
                table: "ChatSessions");

            migrationBuilder.DropTable(
                name: "CitizenProfile");

            migrationBuilder.DropIndex(
                name: "IX_ChatSessions_GovServiceId",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "GovServiceId",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChatSessions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 23, 22, 11, 34, 610, DateTimeKind.Utc).AddTicks(9234), "AQAAAAIAAYagAAAAELctKd8hqCLKZhq4qZg6uF77fo5W1ApOn1bCeCjZeeMjNyxzRTrRTUV2y7s1fu4yaw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 23, 22, 11, 34, 764, DateTimeKind.Utc).AddTicks(1384), "AQAAAAIAAYagAAAAED4rL8vqT8Ug1uFrzY394BXG6jO5jLUZLR51c7ruvh7gt2XEM9jm7VQz1P7maJqsUA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 23, 22, 11, 34, 910, DateTimeKind.Utc).AddTicks(1209), "AQAAAAIAAYagAAAAEI3rUE3D28jPPbhLHyGmb0/HwdB+g7dO5MqUgs9HOLOzS205Kk6ddoIBwlm+M9D/Ag==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 23, 22, 11, 35, 66, DateTimeKind.Utc).AddTicks(3917), "AQAAAAIAAYagAAAAEDb1Ole/xYxu3/K+plFb8DXMiwIEz5C+gdoM60FbCFhfEqclnjYCRmZutOhlIA9eMQ==" });
        }
    }
}
