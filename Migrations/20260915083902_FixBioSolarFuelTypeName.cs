using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fleetops_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixBioSolarFuelTypeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // firstMigration menulis "BioSolar (Subsidi)" ke database, sedangkan
            // seed, pesan validasi, dan dropdown di mobile sama-sama memakai
            // "BioSolar" — akibatnya POST /api/drivers/fuel dengan BioSolar
            // selalu ditolak 404. Selaraskan nilai di database.
            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 2,
                column: "fuel_type",
                value: "BioSolar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 2,
                column: "fuel_type",
                value: "BioSolar (Subsidi)");
        }
    }
}
