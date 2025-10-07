using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BdmiAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddJWT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 7, 19, 46, 0, 400, DateTimeKind.Utc).AddTicks(8157));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 13, 14, 49, 31, 543, DateTimeKind.Utc).AddTicks(8726));
        }
    }
}
