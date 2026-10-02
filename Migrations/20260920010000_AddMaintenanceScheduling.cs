using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fleetops_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_maintenances_vehicle_id",
                table: "maintenances");

            migrationBuilder.AlterColumn<DateTime>(
                name: "tanggal",
                table: "maintenances",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "jenis",
                table: "maintenances",
                type: "text",
                nullable: false,
                defaultValue: "Pemeriksaan rutin");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "overdue_notified_at",
                table: "maintenances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "reminder_sent_at",
                table: "maintenances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "maintenances",
                type: "text",
                nullable: false,
                defaultValue: "Completed");

            migrationBuilder.AddColumn<DateOnly>(
                name: "tanggal_jadwal",
                table: "maintenances",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "target_odometer",
                table: "maintenances",
                type: "integer",
                nullable: true);

            // Baris lama adalah riwayat pemeliharaan yang sudah selesai; default hanya untuk backfill.
            migrationBuilder.AlterColumn<string>(
                name: "jenis",
                table: "maintenances",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Pemeriksaan rutin");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "maintenances",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Completed");

            migrationBuilder.UpdateData(
                table: "maintenances",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "jenis", "overdue_notified_at", "reminder_sent_at", "status", "tanggal_jadwal", "target_odometer" },
                values: new object[] { "Pemeriksaan rutin", null, null, "Completed", null, null });

            migrationBuilder.CreateIndex(
                name: "ix_maintenances_vehicle_id_status",
                table: "maintenances",
                columns: new[] { "vehicle_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_maintenances_vehicle_id_status",
                table: "maintenances");

            migrationBuilder.DropColumn(
                name: "jenis",
                table: "maintenances");

            migrationBuilder.DropColumn(
                name: "overdue_notified_at",
                table: "maintenances");

            migrationBuilder.DropColumn(
                name: "reminder_sent_at",
                table: "maintenances");

            migrationBuilder.DropColumn(
                name: "status",
                table: "maintenances");

            migrationBuilder.DropColumn(
                name: "tanggal_jadwal",
                table: "maintenances");

            migrationBuilder.DropColumn(
                name: "target_odometer",
                table: "maintenances");

            migrationBuilder.AlterColumn<DateTime>(
                name: "tanggal",
                table: "maintenances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_maintenances_vehicle_id",
                table: "maintenances",
                column: "vehicle_id");
        }
    }
}
