using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace fleetops_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelLocationGeography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "lokasi_pengisian",
                table: "fuels",
                type: "geography (Point, 4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 2,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 3,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 4,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 5,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 6,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "fuel_prices",
                keyColumn: "id",
                keyValue: 7,
                column: "created_at",
                value: new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "lokasi_pengisian",
                table: "fuels",
                type: "geometry",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography (Point, 4326)",
                oldNullable: true);

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
        }
    }
}
