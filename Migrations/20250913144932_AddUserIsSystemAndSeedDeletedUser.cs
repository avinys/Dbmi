using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BdmiAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIsSystemAndSeedDeletedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsSystem", "PasswordHash", "Username" },
                values: new object[] { 1, new DateTime(2025, 9, 13, 14, 49, 31, 543, DateTimeKind.Utc).AddTicks(8726), "deleted@system.local", true, null, "deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "Users");
        }
    }
}
