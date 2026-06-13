using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeToRequiredDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                table: "RequiredDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 13, 6, 2, 46, 597, DateTimeKind.Utc).AddTicks(3127), "AQAAAAIAAYagAAAAEF7jn7l4HGwzXEijJ+PWiTHPWMOGZW5nAtBj5seN1rYRWI60e201TI+P/mP+WsE3oA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 13, 6, 2, 46, 751, DateTimeKind.Utc).AddTicks(5546), "AQAAAAIAAYagAAAAEGZ3tTGAwwrPppjV7ZOW5YBGHOjzOIu5kql5SllQ5R9NA4w5RL0WEGK++L1T/hMpow==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 13, 6, 2, 46, 904, DateTimeKind.Utc).AddTicks(1770), "AQAAAAIAAYagAAAAEM5lTi4LHnweu9UsgbSROfkGiklmbXyfqP1ILg2eT3TY9oQ7AqSCOL6v2rfUU81/tA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 13, 6, 2, 47, 56, DateTimeKind.Utc).AddTicks(2066), "AQAAAAIAAYagAAAAEKZhhEtSHL83EJz5V9S+8stNlD7lANXyBep827nbG51L1CDfo2bBAH010uEdCZd+wA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "RequiredDocuments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 12, 6, 55, 37, 688, DateTimeKind.Utc).AddTicks(8380), "AQAAAAIAAYagAAAAEMjg81hSF5F3ZxN72nU4PBTrf7p3bBpfDvpDl9lA1NdvkgznTpUnC5iTzCgUU2JJ4Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 12, 6, 55, 37, 789, DateTimeKind.Utc).AddTicks(386), "AQAAAAIAAYagAAAAEDfhWTn75o4fP33Mq3RkVs/R/iN08IBTi9eVI6huDktohBnkZl+OVPl/OjOrB6l5Fg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 12, 6, 55, 37, 897, DateTimeKind.Utc).AddTicks(6855), "AQAAAAIAAYagAAAAEGxlLh2C/BTcrSescFMI2b5ouIq4Sm6Zins9kHzVWJbl2vLXfM2nXuvFPGXXJFBPpQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 12, 6, 55, 37, 998, DateTimeKind.Utc).AddTicks(5988), "AQAAAAIAAYagAAAAEIUshO+mMrcjPFxnUXe6yJYTBOBMrOs8Ja8SmaFrKRoPRbhkUbjgS3Jlnh6DlXQJbA==" });
        }
    }
}
