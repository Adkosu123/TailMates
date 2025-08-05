using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 8, 5, 14, 4, 34, 258, DateTimeKind.Utc).AddTicks(1358));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 8, 5, 13, 52, 39, 39, DateTimeKind.Utc).AddTicks(9830));
        }
    }
}
