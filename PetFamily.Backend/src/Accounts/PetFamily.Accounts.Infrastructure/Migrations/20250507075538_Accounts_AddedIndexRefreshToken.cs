using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_AddedIndexRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "jti",
                schema: "accounts",
                table: "refresh_sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_refresh_sessions_refresh_token",
                schema: "accounts",
                table: "refresh_sessions",
                column: "refresh_token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_refresh_sessions_refresh_token",
                schema: "accounts",
                table: "refresh_sessions");

            migrationBuilder.DropColumn(
                name: "jti",
                schema: "accounts",
                table: "refresh_sessions");
        }
    }
}
