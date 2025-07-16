using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPetStatusProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetStatus",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateListed", "PetStatus" },
                values: new object[] { new DateTime(2025, 7, 16, 13, 32, 50, 668, DateTimeKind.Utc).AddTicks(6590), 0 });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2,
                column: "PetStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3,
                column: "PetStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 4,
                column: "PetStatus",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetStatus",
                table: "Pets");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 15, 12, 1, 57, 737, DateTimeKind.Utc).AddTicks(1498));
        }
    }
}
