using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataMount.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    logo = table.Column<string>(type: "text", nullable: true),
                    banned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ban_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contacts_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credential_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blocked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    block_reason = table.Column<string>(type: "text", nullable: true),
                    identifier_contact_id = table.Column<Guid>(type: "uuid", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credential_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_credential_accounts_contacts_identifier_contact_id",
                        column: x => x.identifier_contact_id,
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_credential_accounts_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "oauth_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blocked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    block_reason = table.Column<string>(type: "text", nullable: true),
                    identifier_contact_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_token = table.Column<string>(type: "text", nullable: false),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    provider = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_oauth_accounts_contacts_identifier_contact_id",
                        column: x => x.identifier_contact_id,
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_oauth_accounts_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contacts_owner_id",
                table: "contacts",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_credential_accounts_identifier_contact_id_owner_id",
                table: "credential_accounts",
                columns: new[] { "identifier_contact_id", "owner_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_credential_accounts_owner_id",
                table: "credential_accounts",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_identifier_contact_id_owner_id",
                table: "oauth_accounts",
                columns: new[] { "identifier_contact_id", "owner_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_owner_id",
                table: "oauth_accounts",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credential_accounts");

            migrationBuilder.DropTable(
                name: "oauth_accounts");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
