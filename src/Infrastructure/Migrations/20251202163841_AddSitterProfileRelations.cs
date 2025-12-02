using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSitterProfileRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_services", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sitter_profile_photos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sitter_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "varchar(512)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sitter_profile_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_sitter_profile_photos_sitter_profiles",
                        column: x => x.sitter_profile_id,
                        principalTable: "sitter_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sitter_profile_services",
                columns: table => new
                {
                    sitter_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sitter_profile_services", x => new { x.sitter_profile_id, x.service_id });
                    table.ForeignKey(
                        name: "fk_sitter_profile_services_services",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sitter_profile_services_sitter_profiles",
                        column: x => x.sitter_profile_id,
                        principalTable: "sitter_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_services_name",
                table: "services",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_sitter_profile_photos_sitter_profile_id",
                table: "sitter_profile_photos",
                column: "sitter_profile_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sitter_profile_services_service_id",
                table: "sitter_profile_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "ix_sitter_profile_services_sitter_profile_id",
                table: "sitter_profile_services",
                column: "sitter_profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sitter_profile_photos");

            migrationBuilder.DropTable(
                name: "sitter_profile_services");

            migrationBuilder.DropTable(
                name: "services");
        }
    }
}
