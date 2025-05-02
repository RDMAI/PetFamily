using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetFamily.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_AddedVolunteerAccount_AndParticipantAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_admin_accounts_user_id",
                schema: "accounts",
                table: "admin_accounts");

            migrationBuilder.CreateTable(
                name: "participant_accounts",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    favourite_pets = table.Column<List<Guid>>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_participant_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_participant_accounts_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "accounts",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volunteer_accounts",
                schema: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    experience = table.Column<float>(type: "real", nullable: false),
                    requisites = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_volunteer_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_volunteer_accounts_users_user_id",
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
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_participant_accounts_user_id",
                schema: "accounts",
                table: "participant_accounts",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_volunteer_accounts_user_id",
                schema: "accounts",
                table: "volunteer_accounts",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "participant_accounts",
                schema: "accounts");

            migrationBuilder.DropTable(
                name: "volunteer_accounts",
                schema: "accounts");

            migrationBuilder.DropIndex(
                name: "ix_admin_accounts_user_id",
                schema: "accounts",
                table: "admin_accounts");

            migrationBuilder.CreateIndex(
                name: "ix_admin_accounts_user_id",
                schema: "accounts",
                table: "admin_accounts",
                column: "user_id");
        }
    }
}
