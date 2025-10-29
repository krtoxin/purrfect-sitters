using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseRowVersionForBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "version",
                table: "bookings");

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "bookings",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "decode('0000000000000000','hex')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "row_version",
                table: "bookings");

            migrationBuilder.AddColumn<long>(
                name: "version",
                table: "bookings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
