using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeTiersAndImportantNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "GovServices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "NeedsGuarantee",
                table: "GovServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProviderEntity",
                table: "GovServices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetAudience",
                table: "GovServices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ServiceFeeTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRefundable = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFeeTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFeeTiers_GovServices_GovServiceId",
                        column: x => x.GovServiceId,
                        principalTable: "GovServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceImportantNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceImportantNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceImportantNotes_GovServices_GovServiceId",
                        column: x => x.GovServiceId,
                        principalTable: "GovServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 5, 18, 19, 20, 865, DateTimeKind.Utc).AddTicks(7761), "AQAAAAIAAYagAAAAELWysayBUhLrcIwTUzDgKPY3AorJZwTFLQBooRkyyrhtB4RLjfH6ThQo5ycg17nm3w==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 5, 18, 19, 21, 31, DateTimeKind.Utc).AddTicks(7915), "AQAAAAIAAYagAAAAEFpPg+xkY/aOKUYPVQkw3Ex1j6csejEkDg1xzduIcy+GgiXrK0tLYIiefYmL6791Zw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 5, 18, 19, 21, 197, DateTimeKind.Utc).AddTicks(2618), "AQAAAAIAAYagAAAAEMsj4ei2696yneKErHse5KLU4TLGAhFiCVmG+QH9KVhHCUM3Sn1akqgeY+jSNybYQg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 5, 18, 19, 21, 365, DateTimeKind.Utc).AddTicks(8804), "AQAAAAIAAYagAAAAEL2icm+3DLiq/5qF6EGina/+J8ShoEma+kSlzmUFxxNqs7S3IossIO0tPpb9otVN4A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 5, 18, 19, 21, 531, DateTimeKind.Utc).AddTicks(6968), "AQAAAAIAAYagAAAAEGO1kZmIIdzFqrvv9cSM6+msD3RnpWYUei2PHs3wOIuK6BGoYGjVL4KZos0nF2vUgQ==" });

            migrationBuilder.UpdateData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeliveryMethod", "NeedsGuarantee", "ProviderEntity", "TargetAudience" },
                values: new object[] { "", false, "", "" });

            migrationBuilder.UpdateData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeliveryMethod", "NeedsGuarantee", "ProviderEntity", "TargetAudience" },
                values: new object[] { "", false, "", "" });

            migrationBuilder.UpdateData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DeliveryMethod", "NeedsGuarantee", "ProviderEntity", "TargetAudience" },
                values: new object[] { "", false, "", "" });

            migrationBuilder.UpdateData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeliveryMethod", "NeedsGuarantee", "ProviderEntity", "TargetAudience" },
                values: new object[] { "", false, "", "" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeeTiers_GovServiceId",
                table: "ServiceFeeTiers",
                column: "GovServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceImportantNotes_GovServiceId",
                table: "ServiceImportantNotes",
                column: "GovServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceFeeTiers");

            migrationBuilder.DropTable(
                name: "ServiceImportantNotes");

            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "GovServices");

            migrationBuilder.DropColumn(
                name: "NeedsGuarantee",
                table: "GovServices");

            migrationBuilder.DropColumn(
                name: "ProviderEntity",
                table: "GovServices");

            migrationBuilder.DropColumn(
                name: "TargetAudience",
                table: "GovServices");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 30, 16, 17, 54, 465, DateTimeKind.Utc).AddTicks(5612), "AQAAAAIAAYagAAAAEH9HOaMPJdpUqxeyby898nYTM4/51w0NzIoWhSAZqxin0P3MzAXeQMXZRWIZvEatsQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 30, 16, 17, 54, 571, DateTimeKind.Utc).AddTicks(6748), "AQAAAAIAAYagAAAAEDaQAmE0mZaXOInp1LsCPoTB2RI9om6s6LnDA+L1cmp9urleQSRVhkEQZX/nKk+OoA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 30, 16, 17, 54, 681, DateTimeKind.Utc).AddTicks(5677), "AQAAAAIAAYagAAAAEMjYuaE+dBN2KvGXkg5FJLqogacRrfZYr/LafgkP/MSdkHYPrdclJ5s5nqjHfVlapg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 30, 16, 17, 54, 787, DateTimeKind.Utc).AddTicks(6752), "AQAAAAIAAYagAAAAEFlMCg//1ervFMaUs+mDSq0ru8IrIz9521MBznO76BPQfjcKwbp7YCdTxBO40E3ljg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 30, 16, 17, 54, 896, DateTimeKind.Utc).AddTicks(7220), "AQAAAAIAAYagAAAAEOeof9VsqG1w6VFznMD1S5PC8lLqx3zDA8aQPsadGJN63UEqHSKRNV/HNl6ASIpjKw==" });
        }
    }
}
