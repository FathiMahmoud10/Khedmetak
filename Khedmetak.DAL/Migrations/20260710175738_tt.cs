using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 17, 57, 35, 931, DateTimeKind.Utc).AddTicks(1673), "AQAAAAIAAYagAAAAEN/tLZQEHf1G2LhW8r9ZsPULg22N3ZkdqDhzX+cXA7SmlwTbey0j1nopNBtgx6O2bg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 17, 57, 36, 11, DateTimeKind.Utc).AddTicks(1622), "AQAAAAIAAYagAAAAEFaW0ugdSAONljx8Jtx+d8uXLs0ANyvTHgbRaCSLbB3vKB5w7NfgQ/2KxjYSjO2aOg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 17, 57, 36, 110, DateTimeKind.Utc).AddTicks(5849), "AQAAAAIAAYagAAAAEM2UOugKzTy5eSmfitkp3T3HdiV9Hem92IibmqCFmrxEhs10r+IVx7l4gz7dS8Es2g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 17, 57, 36, 203, DateTimeKind.Utc).AddTicks(6172), "AQAAAAIAAYagAAAAEOZZ7pwC5BP0YNN424CXmRQhSNaQrJ6B2ZQOa1AXQAyD9b3lL2BKv8+dE+rseHJMpw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 17, 57, 36, 290, DateTimeKind.Utc).AddTicks(7536), "AQAAAAIAAYagAAAAEJR6eSiqjw1JXpIvz+T5xG3ufpM+t0Vt5IJgvsKgq2i1XbwerIKMVMjTOMzBHRTngQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
