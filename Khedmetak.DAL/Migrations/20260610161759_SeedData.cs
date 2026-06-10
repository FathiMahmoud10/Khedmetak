using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "الأحوال المدنية" },
                    { 2, "المرور" },
                    { 3, "التعليم" },
                    { 4, "الصحة" },
                    { 5, "التموين" }
                });

            migrationBuilder.InsertData(
                table: "GovServices",
                columns: new[] { "Id", "CategoryId", "EstimatedFees", "SrvDesc", "SrvFees", "SrvName", "SrvTime" },
                values: new object[,]
                {
                    { 1, 1, 50m, "إصدار بطاقة رقم قومي لأول مرة", 50m, "استخراج بطاقة رقم قومي", "7 أيام" },
                    { 2, 1, 50m, "تجديد بطاقة الرقم القومي المنتهية", 50m, "تجديد بطاقة رقم قومي", "3 أيام" },
                    { 3, 2, 500m, "تجديد رخصة المركبة", 500m, "تجديد رخصة سيارة", "يوم واحد" },
                    { 4, 1, 30m, "إصدار شهادة ميلاد بدل فاقد", 30m, "استخراج بدل فاقد شهادة ميلاد", "فوري" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GovServices",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
