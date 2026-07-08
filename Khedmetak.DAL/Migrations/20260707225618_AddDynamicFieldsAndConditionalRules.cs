using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicFieldsAndConditionalRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConditionalRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    DependentOnType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DependentOnId = table.Column<int>(type: "int", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionalRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConditionalRules_GovServices_GovServiceId",
                        column: x => x.GovServiceId,
                        principalTable: "GovServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceFormFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Placeholder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FieldType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Choices = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ValidationRegex = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    GovServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFormFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFormFields_GovServices_GovServiceId",
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

            migrationBuilder.CreateIndex(
                name: "IX_ConditionalRules_GovServiceId",
                table: "ConditionalRules",
                column: "GovServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFormFields_GovServiceId",
                table: "ServiceFormFields",
                column: "GovServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConditionalRules");

            migrationBuilder.DropTable(
                name: "ServiceFormFields");

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
        }
    }
}
