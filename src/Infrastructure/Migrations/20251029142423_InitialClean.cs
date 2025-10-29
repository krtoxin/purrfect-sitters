using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sitter_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    price_base_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    price_service_fee_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    price_service_fee_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    price_total_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    price_currency = table.Column<string>(type: "varchar(3)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<int>(type: "integer", nullable: true),
                    service_type = table.Column<int>(type: "integer", nullable: false),
                    is_reviewed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "owner_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_care_notes = table.Column<string>(type: "varchar(800)", nullable: true),
                    address_line1 = table.Column<string>(type: "varchar(200)", nullable: true),
                    address_line2 = table.Column<string>(type: "varchar(200)", nullable: true),
                    address_city = table.Column<string>(type: "varchar(120)", nullable: true),
                    address_state = table.Column<string>(type: "varchar(120)", nullable: true),
                    address_postal_code = table.Column<string>(type: "varchar(40)", nullable: true),
                    address_country = table.Column<string>(type: "varchar(80)", nullable: true),
                    preferred_timezone = table.Column<string>(type: "varchar(100)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_owner_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", nullable: false),
                    type = table.Column<string>(type: "varchar(50)", nullable: false),
                    breed = table.Column<string>(type: "varchar(150)", nullable: true),
                    notes = table.Column<string>(type: "varchar(500)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sitter_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bio = table.Column<string>(type: "varchar(800)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    average_rating = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false, defaultValue: 0m),
                    base_rate_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    base_rate_currency = table.Column<string>(type: "varchar(3)", nullable: true),
                    services_offered = table.Column<int>(type: "integer", nullable: false),
                    completed_bookings = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sitter_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sitters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    address_line1 = table.Column<string>(type: "text", nullable: false),
                    address_line2 = table.Column<string>(type: "text", nullable: true),
                    address_city = table.Column<string>(type: "text", nullable: false),
                    address_state = table.Column<string>(type: "text", nullable: false),
                    address_postal_code = table.Column<string>(type: "text", nullable: false),
                    address_country = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    hourly_rate_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    hourly_rate_currency = table.Column<string>(type: "text", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sitters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "varchar(255)", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    roles = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "booking_care_instruction_snapshots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "varchar(400)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_care_instruction_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_care_instruction_snapshots_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking_status_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    changed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_status_history_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "owner_emergency_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", nullable: false),
                    phone = table.Column<string>(type: "varchar(60)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_owner_emergency_contacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_owner_emergency_contacts_owner_profiles_owner_profile_id",
                        column: x => x.owner_profile_id,
                        principalTable: "owner_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_allergies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "varchar(500)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pet_allergies", x => x.id);
                    table.ForeignKey(
                        name: "FK_pet_allergies_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_instructions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "varchar(400)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pet_instructions", x => x.id);
                    table.ForeignKey(
                        name: "FK_pet_instructions_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_medical_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "varchar(200)", nullable: false),
                    details = table.Column<string>(type: "varchar(1000)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pet_medical_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_pet_medical_entries_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_medications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", nullable: false),
                    dosage = table.Column<string>(type: "varchar(150)", nullable: false),
                    schedule = table.Column<string>(type: "varchar(250)", nullable: false),
                    start_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pet_medications", x => x.id);
                    table.ForeignKey(
                        name: "FK_pet_medications_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_vaccinations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_name = table.Column<string>(type: "varchar(200)", nullable: false),
                    administered_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pet_vaccinations", x => x.id);
                    table.ForeignKey(
                        name: "FK_pet_vaccinations_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sitter_availability_slots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sitter_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sitter_availability_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_sitter_availability_slots_sitter_profiles_sitter_profile_id",
                        column: x => x.sitter_profile_id,
                        principalTable: "sitter_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_booking_care_instruction_snapshots_booking_id",
                table: "booking_care_instruction_snapshots",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_booking_status_history_booking_id",
                table: "booking_status_history",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_owner_created",
                table: "bookings",
                columns: new[] { "owner_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_pet_id",
                table: "bookings",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_sitter_period",
                table: "bookings",
                columns: new[] { "sitter_profile_id", "start_utc", "end_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_status",
                table: "bookings",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_owner_emergency_contacts_owner_profile_id",
                table: "owner_emergency_contacts",
                column: "owner_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_owner_profiles_is_active",
                table: "owner_profiles",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_owner_profiles_user_id",
                table: "owner_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pet_allergies_pet_id",
                table: "pet_allergies",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_pet_instructions_pet_id",
                table: "pet_instructions",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_pet_medical_entries_pet_id",
                table: "pet_medical_entries",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_pet_medications_pet_id",
                table: "pet_medications",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_pet_vaccinations_pet_id",
                table: "pet_vaccinations",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_owner_id",
                table: "pets",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_pets_owner_type",
                table: "pets",
                columns: new[] { "owner_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_sitter_availability_slots_profile_id",
                table: "sitter_availability_slots",
                column: "sitter_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_sitter_availability_slots_range",
                table: "sitter_availability_slots",
                columns: new[] { "start_utc", "end_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_sitter_profiles_is_active",
                table: "sitter_profiles",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_sitter_profiles_user_id",
                table: "sitter_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_care_instruction_snapshots");

            migrationBuilder.DropTable(
                name: "booking_status_history");

            migrationBuilder.DropTable(
                name: "owner_emergency_contacts");

            migrationBuilder.DropTable(
                name: "pet_allergies");

            migrationBuilder.DropTable(
                name: "pet_instructions");

            migrationBuilder.DropTable(
                name: "pet_medical_entries");

            migrationBuilder.DropTable(
                name: "pet_medications");

            migrationBuilder.DropTable(
                name: "pet_vaccinations");

            migrationBuilder.DropTable(
                name: "sitter_availability_slots");

            migrationBuilder.DropTable(
                name: "Sitters");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "owner_profiles");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "sitter_profiles");
        }
    }
}
