using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManagedShelterToUserAndDefaultManagerSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Pets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManagedShelterId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateListed", "IsApproved" },
                values: new object[] { new DateTime(2025, 7, 14, 8, 54, 23, 786, DateTimeKind.Utc).AddTicks(218), false });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsApproved",
                value: false);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsApproved",
                value: false);

            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "Age", "BreedId", "DateListed", "Description", "Gender", "ImageUrl", "IsAdopted", "IsApproved", "Name", "ShelterId", "SpeciesId" },
                values: new object[] { 4, 4, 4, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A shy but loving black cat, needs a quiet home.", "Female", "https://th.bing.com/th/id/R.1480d0e8449c4676cb50b3d691f89b39?rik=sDKV6DJKqVKHUw&pid=ImgRaw&r=0", false, false, "Shadow", 1, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ManagedShelterId",
                table: "AspNetUsers",
                column: "ManagedShelterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Shelters_ManagedShelterId",
                table: "AspNetUsers",
                column: "ManagedShelterId",
                principalTable: "Shelters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Shelters_ManagedShelterId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ManagedShelterId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "ManagedShelterId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 14, 8, 38, 5, 957, DateTimeKind.Utc).AddTicks(4614));
        }
    }
}
