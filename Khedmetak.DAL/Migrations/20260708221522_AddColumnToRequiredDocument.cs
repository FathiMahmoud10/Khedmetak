using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnToRequiredDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecificRule",
                table: "RequiredDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 15, 21, 616, DateTimeKind.Utc).AddTicks(8458), "AQAAAAIAAYagAAAAEGOi4MQEvjBBPEYY27Kvrxh1cEsQKmXrSARO46zUPZbfp63UrWwC5JaP7u6tnLIVxg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 15, 21, 707, DateTimeKind.Utc).AddTicks(9537), "AQAAAAIAAYagAAAAEK95TYW9XHn7JFBAI2vQBKDv8Dkop5VhWoGLuJOvcgzP7ooeP/XrutSg//SOOezPrg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 15, 21, 811, DateTimeKind.Utc).AddTicks(4355), "AQAAAAIAAYagAAAAEPZaUBrNwFWSd20PaJ5q5+SioGXSXoPqSdmjAEf6/fT98/jjcMlmD5SZtDAGUczoqg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 15, 21, 901, DateTimeKind.Utc).AddTicks(8793), "AQAAAAIAAYagAAAAEPUpNKC2LJ3jkfHG3PbLOPvSy8tlDQoaeej0Oas3/mAx7uLn+I1Yd/L269W3QBT6Qw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 15, 21, 988, DateTimeKind.Utc).AddTicks(8345), "AQAAAAIAAYagAAAAEJj2zvS0bSwscHaGoOnUoZa2dNwVSgYVAQri+gRlnacPVPslL7oSrUpc1EoQcrqdWQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificRule",
                table: "RequiredDocuments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 580, DateTimeKind.Utc).AddTicks(6376), "AQAAAAIAAYagAAAAECs4+e5Hpbc1XIZ2OjaU/sl3fq8A9qEAj2gbmbCHTspUhBMq4jUiOYtB9me0k5HcmA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 689, DateTimeKind.Utc).AddTicks(6988), "AQAAAAIAAYagAAAAEO2U4hjjV9DWCz0UB2wRB/z/BHkWxnunJaxxRdWKJTlE1Ix4jSR/kXsPplWlGt6yKg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 786, DateTimeKind.Utc).AddTicks(6551), "AQAAAAIAAYagAAAAEM5dRbFge1fpdcVaPl+SxaZ0D2/q3v1aBB9kwxPcCz6vPEJUzqM4eSVXJLuH5wPUYw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 878, DateTimeKind.Utc).AddTicks(8918), "AQAAAAIAAYagAAAAEGjNyTMOpPH9tqpmUi6knJseo9f1BS5BbPLVUASeLFcj0rQIf7yexZgKg1T+zUQeYQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 8, 22, 12, 22, 986, DateTimeKind.Utc).AddTicks(6402), "AQAAAAIAAYagAAAAELzLNLFffOmd5Z0QBDQbwWMWhoNsi2o7Ni/OfuETX6c5p9h7L31KUWgEBqf+NHetSg==" });
        }
    }
}
