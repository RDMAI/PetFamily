using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteVolunteersAndPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets");

            migrationBuilder.AddColumn<DateTime>(
                name: "deletion_date",
                table: "Volunteers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Volunteers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deletion_date",
                table: "Pets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Pets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets",
                column: "volunteer_id",
                principalTable: "Volunteers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "deletion_date",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Volunteers");

            migrationBuilder.DropColumn(
                name: "deletion_date",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Pets");

            migrationBuilder.AddForeignKey(
                name: "fk_pets_volunteers_volunteer_id",
                table: "Pets",
                column: "volunteer_id",
                principalTable: "Volunteers",
                principalColumn: "id");
        }
    }
}
