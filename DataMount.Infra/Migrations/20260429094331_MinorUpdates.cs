using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataMount.Infra.Migrations
{
    /// <inheritdoc />
    public partial class MinorUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_login_attempts_accounts_account_id",
                table: "login_attempts");

            migrationBuilder.DropIndex(
                name: "ix_accounts_identifier_contact_id_owner_id",
                table: "accounts");

            migrationBuilder.AddColumn<string>(
                name: "ip",
                table: "login_attempts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_agent",
                table: "login_attempts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "contacts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_contacts_type_value",
                table: "contacts",
                columns: new[] { "type", "value" });

            migrationBuilder.AddUniqueConstraint(
                name: "ak_accounts_identifier_contact_id_owner_id",
                table: "accounts",
                columns: new[] { "identifier_contact_id", "owner_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_login_attempts_account_guid_account_id",
                table: "login_attempts",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_login_attempts_account_guid_account_id",
                table: "login_attempts");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_contacts_type_value",
                table: "contacts");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_accounts_identifier_contact_id_owner_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "ip",
                table: "login_attempts");

            migrationBuilder.DropColumn(
                name: "user_agent",
                table: "login_attempts");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_identifier_contact_id_owner_id",
                table: "accounts",
                columns: new[] { "identifier_contact_id", "owner_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_login_attempts_accounts_account_id",
                table: "login_attempts",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
