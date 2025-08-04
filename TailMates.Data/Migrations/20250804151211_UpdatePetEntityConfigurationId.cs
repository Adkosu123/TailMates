using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetEntityConfigurationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 8, 4, 15, 12, 9, 365, DateTimeKind.Utc).AddTicks(5336));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 21, 12, 26, 30, 494, DateTimeKind.Utc).AddTicks(9480));
        }
    }
}
