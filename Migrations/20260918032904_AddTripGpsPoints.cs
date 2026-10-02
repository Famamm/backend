using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace fleetops_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTripGpsPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trip_gps_points",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    coordinate = table.Column<Point>(type: "geography (Point, 4326)", nullable: false),
                    captured_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    accuracy_meters = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_gps_points", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_gps_points_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7544));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7952));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7954));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7956));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 5,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7957));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 6,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7959));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 7,
                column: "created_at",
                value: new DateTime(2026, 9, 18, 3, 29, 2, 507, DateTimeKind.Utc).AddTicks(7961));

            migrationBuilder.CreateIndex(
                name: "ix_trip_gps_points_trip_id_client_id",
                table: "trip_gps_points",
                columns: new[] { "trip_id", "client_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_gps_points");

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(720));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(841));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(842));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(843));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 5,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(844));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 6,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(845));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 7,
                column: "created_at",
                value: new DateTime(2026, 9, 15, 8, 39, 2, 288, DateTimeKind.Utc).AddTicks(846));
        }
    }
}
