using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_AddedAdminAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_accounts",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_admin_accounts_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "accounts",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admin_accounts_user_id",
                schema: "accounts",
                table: "admin_accounts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_accounts",
                schema: "accounts");
        }
    }
}
