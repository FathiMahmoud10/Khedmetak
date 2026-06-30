using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khedmetak.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalIdToCitizenProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedViaDigitalPortal",
                table: "CitizenProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "CitizenProfiles",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerifiedViaDigitalPortal",
                table: "CitizenProfiles");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "CitizenProfiles");

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
        }
    }
}
