using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataMount.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "projects",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "forms",
                newName: "organization_id");

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_id",
                table: "projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_id",
                table: "forms",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "org_memberships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    permission_string = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_org_memberships", x => x.id);
                    table.ForeignKey(
                        name: "fk_org_memberships_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_org_memberships_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_projects_created_by_id",
                table: "projects",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_organization_id",
                table: "projects",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_forms_created_by_id",
                table: "forms",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_forms_organization_id",
                table: "forms",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_org_memberships_organization_id",
                table: "org_memberships",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_org_memberships_user_id_organization_id",
                table: "org_memberships",
                columns: new[] { "user_id", "organization_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_forms_organization_memberships_created_by_id",
                table: "forms",
                column: "created_by_id",
                principalTable: "org_memberships",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_forms_organizations_organization_id",
                table: "forms",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_organization_memberships_created_by_id",
                table: "projects",
                column: "created_by_id",
                principalTable: "org_memberships",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_organizations_organization_id",
                table: "projects",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_forms_organization_memberships_created_by_id",
                table: "forms");

            migrationBuilder.DropForeignKey(
                name: "fk_forms_organizations_organization_id",
                table: "forms");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_organization_memberships_created_by_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_organizations_organization_id",
                table: "projects");

            migrationBuilder.DropTable(
                name: "org_memberships");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropIndex(
                name: "ix_projects_created_by_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "ix_projects_organization_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "ix_forms_created_by_id",
                table: "forms");

            migrationBuilder.DropIndex(
                name: "ix_forms_organization_id",
                table: "forms");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "forms");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "projects",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "forms",
                newName: "created_by");
        }
    }
}
