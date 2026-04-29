using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataMount.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RenameLogoToPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sessions_accounts_account_id",
                table: "sessions");

            migrationBuilder.RenameColumn(
                name: "logo",
                table: "users",
                newName: "photo");

            migrationBuilder.AddForeignKey(
                name: "fk_sessions_account_guid_account_id",
                table: "sessions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sessions_account_guid_account_id",
                table: "sessions");

            migrationBuilder.RenameColumn(
                name: "photo",
                table: "users",
                newName: "logo");

            migrationBuilder.AddForeignKey(
                name: "fk_sessions_accounts_account_id",
                table: "sessions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
