using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidIdToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionGuid",
                table: "ChatSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 21, 3, 0, 625, DateTimeKind.Utc).AddTicks(1959), "AQAAAAIAAYagAAAAEF+WB5RguYYM66PFIIGWautHk3SzGHPkIIPM+jNsFAy9DVxlpR8iP12feLChp590Jw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 21, 3, 0, 701, DateTimeKind.Utc).AddTicks(6323), "AQAAAAIAAYagAAAAEItwMX45ZnsoMHMX9ao9asvJ2hj+44cJCcFqVePMDnOPdXQ0lKo9NgHVWDjnJ645aw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 21, 3, 0, 781, DateTimeKind.Utc).AddTicks(6526), "AQAAAAIAAYagAAAAEFXtG7MnG79MbxP8OP8Xauw4m5a99FKw96BQAYQuTP84+f9DhSdAcQL9LZ+OPTFbIQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 14, 21, 3, 0, 864, DateTimeKind.Utc).AddTicks(6980), "AQAAAAIAAYagAAAAEKu7FPNJBfpcqDdXtY/QU2oPiacsepmHnfi2WTac1pd2PvOpi6lnWATI9CAdDxkWqA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionGuid",
                table: "ChatSessions");

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
    }
}
