using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace fleetops_backend.Migrations
{
    /// <inheritdoc />
    public partial class firstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "fuel_prices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fuel_type = table.Column<string>(type: "text", nullable: false),
                    price_per_liter = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fuel_prices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inspection_item_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_item_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inspection_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    inspection_item_category_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_inspection_items_inspection_item_categories_inspection_item",
                        column: x => x.inspection_item_category_id,
                        principalTable: "inspection_item_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    profile_photo = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "drivers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    jenis_lisensi = table.Column<string>(type: "text", nullable: true),
                    tanggal_berlaku = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    foto_lisensi = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drivers", x => x.id);
                    table.ForeignKey(
                        name: "fk_drivers_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_drivers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<int>(type: "integer", nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_name = table.Column<string>(type: "text", nullable: false),
                    plat_nomor = table.Column<string>(type: "text", nullable: false),
                    tahun_pembuatan = table.Column<int>(type: "integer", nullable: false),
                    odometer = table.Column<int>(type: "integer", nullable: false),
                    tipe = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    muatan_max = table.Column<int>(type: "integer", nullable: false),
                    tipe_bensin = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicles_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vehicles_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fuels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    driver_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    fuel_price_id = table.Column<int>(type: "integer", nullable: false),
                    jml_liter = table.Column<decimal>(type: "numeric", nullable: false),
                    harga_total = table.Column<int>(type: "integer", nullable: false),
                    lokasi_pengisian = table.Column<Point>(type: "geometry", nullable: true),
                    odometer_isi = table.Column<int>(type: "integer", nullable: false),
                    image_url_path = table.Column<string>(type: "text", nullable: true),
                    catatan = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fuels", x => x.id);
                    table.ForeignKey(
                        name: "fk_fuels_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_fuels_fuel_prices_fuel_price_id",
                        column: x => x.fuel_price_id,
                        principalTable: "fuel_prices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fuels_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_fuels_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inspections",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspections", x => x.id);
                    table.ForeignKey(
                        name: "fk_inspections_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inspections_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inspections_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "maintenances",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    tanggal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    catatan = table.Column<string>(type: "text", nullable: true),
                    odometer = table.Column<int>(type: "integer", nullable: true),
                    biaya = table.Column<decimal>(type: "numeric", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenances", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenances_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_maintenances_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    driver_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    kord_awal = table.Column<Point>(type: "geography (Point, 4326)", nullable: true),
                    kord_akhir = table.Column<Point>(type: "geography (Point, 4326)", nullable: true),
                    lokasi_awal = table.Column<string>(type: "text", nullable: true),
                    lokasi_akhir = table.Column<string>(type: "text", nullable: true),
                    odometer_awal = table.Column<int>(type: "integer", nullable: true),
                    odometer_akhir = table.Column<int>(type: "integer", nullable: true),
                    waktu_mulai = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    waktu_selesai = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    catatan = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    organization_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trips", x => x.id);
                    table.ForeignKey(
                        name: "fk_trips_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trips_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trips_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inspection_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    inspection_id = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    image_url_path = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_inspection_photos_inspections_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    trip_id = table.Column<int>(type: "integer", nullable: false),
                    jarak = table.Column<int>(type: "integer", nullable: false),
                    avg_speed = table.Column<int>(type: "integer", nullable: false),
                    waktu = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trip_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_details_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inspection_results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    inspection_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    photo_id = table.Column<int>(type: "integer", nullable: true),
                    condition = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inspection_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_inspection_results_inspection_items_item_id",
                        column: x => x.item_id,
                        principalTable: "inspection_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inspection_results_inspection_photos_photo_id",
                        column: x => x.photo_id,
                        principalTable: "inspection_photos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_inspection_results_inspections_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "fuel_prices",
                columns: new[] { "id", "created_at", "fuel_type", "price_per_liter", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(5650), "Pertalite", 10000, null },
                    { 2, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(5965), "BioSolar (Subsidi)", 6800, null },
                    { 3, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(5967), "Pertamax (RON 92)", 16250, null },
                    { 4, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(5968), "Pertamax Green 95", 17000, null },
                    { 5, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(6013), "Pertamax Turbo (RON 98)", 20750, null },
                    { 6, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(6014), "Dexlite", 23000, null },
                    { 7, new DateTime(2026, 6, 13, 10, 29, 0, 880, DateTimeKind.Utc).AddTicks(6015), "Pertamina Dex", 27900, null }
                });

            migrationBuilder.InsertData(
                table: "inspection_item_categories",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Eksterior" },
                    { 2, "Interior" },
                    { 3, "Mesin" },
                    { 4, "Kelengkapan" }
                });

            migrationBuilder.InsertData(
                table: "organizations",
                columns: new[] { "id", "created_at", "organization_name" },
                values: new object[] { 1, new DateTimeOffset(new DateTime(2026, 5, 29, 9, 38, 19, 734, DateTimeKind.Unspecified).AddTicks(4434), new TimeSpan(0, 0, 0, 0, 0)), "FleetOps Organization" });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "FleetManager" },
                    { 3, "Driver" },
                    { 4, "Employee" },
                    { 5, "Unassigned" }
                });

            migrationBuilder.InsertData(
                table: "inspection_items",
                columns: new[] { "id", "inspection_item_category_id", "is_required", "name" },
                values: new object[,]
                {
                    { 1, 1, true, "Ban" },
                    { 2, 1, true, "Lampu" },
                    { 3, 1, true, "Spion" },
                    { 4, 1, true, "Wiper" },
                    { 5, 1, true, "Body Kendaraan" },
                    { 6, 2, true, "Sabuk pengaman" },
                    { 7, 2, true, "Kaca depan" },
                    { 8, 2, true, "AC" },
                    { 9, 2, true, "Indikator" },
                    { 10, 2, true, "Klakson" },
                    { 11, 3, true, "Oli mesin" },
                    { 12, 3, true, "Air radiator" },
                    { 13, 3, true, "Aki" },
                    { 14, 3, true, "Selang radiator" },
                    { 15, 3, true, "Kebocoran oli" },
                    { 16, 4, true, "KIR kendaraan" },
                    { 17, 4, true, "Segitiga pengaman" },
                    { 18, 4, true, "Kotak P3K" },
                    { 19, 4, true, "Ban cadangan" },
                    { 20, 4, true, "APAR" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "address", "created_at", "email", "full_name", "organization_id", "password_hash", "profile_photo", "role_id", "username" },
                values: new object[] { 1, "Jl. Test No. 1, Jakarta", new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "driver@fleetops.local", "FleetOps Test Driver", 1, "$2a$11$wvBoSd9TRmWoarbcfmSrwu.AeqYvmWSP1o.fdUrj88yS33TSJL2Dm", "https://example.com/profile/driver1.jpg", 3, "fleetops.test.driver" });

            migrationBuilder.InsertData(
                table: "drivers",
                columns: new[] { "id", "foto_lisensi", "jenis_lisensi", "organization_id", "status", "tanggal_berlaku", "user_id" },
                values: new object[] { 1, "https://example.com/sim/driver1.jpg", "B", 1, "Active", new DateTimeOffset(new DateTime(2031, 5, 2, 11, 59, 13, 649, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1 });

            migrationBuilder.InsertData(
                table: "vehicles",
                columns: new[] { "id", "driver_id", "image_url", "muatan_max", "odometer", "organization_id", "plat_nomor", "status", "tahun_pembuatan", "tipe", "tipe_bensin", "vehicle_name" },
                values: new object[] { 1, 1, null, 1000, 0, 1, "B 1234 ABC", "Active", 2024, "Truck", 0, "Truck-001" });

            migrationBuilder.InsertData(
                table: "fuels",
                columns: new[] { "id", "catatan", "created_at", "driver_id", "fuel_price_id", "harga_total", "image_url_path", "jml_liter", "lokasi_pengisian", "odometer_isi", "organization_id", "vehicle_id" },
                values: new object[] { 1, null, new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc), 1, 3, 750000, null, 50m, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (106.8272 -6.1751)"), 0, 1, 1 });

            migrationBuilder.InsertData(
                table: "inspections",
                columns: new[] { "id", "created_at", "driver_id", "organization_id", "status", "vehicle_id" },
                values: new object[] { 1, new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 652, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 1, true, 1 });

            migrationBuilder.InsertData(
                table: "maintenances",
                columns: new[] { "id", "biaya", "catatan", "created_at", "odometer", "organization_id", "tanggal", "vehicle_id" },
                values: new object[] { 1, 500000m, "Rutin maintenance check", new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5000, 1, new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "trips",
                columns: new[] { "id", "catatan", "driver_id", "kord_akhir", "kord_awal", "lokasi_akhir", "lokasi_awal", "odometer_akhir", "odometer_awal", "organization_id", "status", "vehicle_id", "waktu_mulai", "waktu_selesai" },
                values: new object[] { 1, "Trip completed successfully", 1, null, null, "Bandung", "Jakarta", 1150, 1000, 1, "Finished", 1, new DateTimeOffset(new DateTime(2026, 5, 2, 9, 59, 13, 650, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 650, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "inspection_photos",
                columns: new[] { "id", "image_url_path", "inspection_id", "is_required" },
                values: new object[] { 1, "https://example.com/photo1.jpg", 1, true });

            migrationBuilder.InsertData(
                table: "inspection_results",
                columns: new[] { "id", "condition", "inspection_id", "item_id", "note", "photo_id" },
                values: new object[] { 2, 1, 1, 2, "Lampu berfungsi normal", null });

            migrationBuilder.InsertData(
                table: "trip_details",
                columns: new[] { "id", "avg_speed", "image_url", "jarak", "trip_id", "waktu" },
                values: new object[] { 1, 75, "https://example.com/driver-image.jpg", 150, 1, 7200 });

            migrationBuilder.InsertData(
                table: "inspection_results",
                columns: new[] { "id", "condition", "inspection_id", "item_id", "note", "photo_id" },
                values: new object[] { 1, 1, 1, 1, "Ban dalam kondisi baik", 1 });

            migrationBuilder.CreateIndex(
                name: "ix_drivers_organization_id",
                table: "drivers",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_drivers_user_id",
                table: "drivers",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fuels_driver_id",
                table: "fuels",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuels_fuel_price_id",
                table: "fuels",
                column: "fuel_price_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuels_organization_id",
                table: "fuels",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_fuels_vehicle_id",
                table: "fuels",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_items_inspection_item_category_id",
                table: "inspection_items",
                column: "inspection_item_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_photos_inspection_id",
                table: "inspection_photos",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_results_inspection_id",
                table: "inspection_results",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_results_item_id",
                table: "inspection_results",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspection_results_photo_id",
                table: "inspection_results",
                column: "photo_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspections_driver_id",
                table: "inspections",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspections_organization_id",
                table: "inspections",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_inspections_vehicle_id",
                table: "inspections",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenances_organization_id",
                table: "maintenances",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenances_vehicle_id",
                table: "maintenances",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_trip_details_trip_id",
                table: "trip_details",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "ix_trips_driver_id",
                table: "trips",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_trips_organization_id",
                table: "trips",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_trips_vehicle_id",
                table: "trips",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_organization_id",
                table: "users",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_driver_id",
                table: "vehicles",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_organization_id",
                table: "vehicles",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fuels");

            migrationBuilder.DropTable(
                name: "inspection_results");

            migrationBuilder.DropTable(
                name: "maintenances");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "trip_details");

            migrationBuilder.DropTable(
                name: "fuel_prices");

            migrationBuilder.DropTable(
                name: "inspection_items");

            migrationBuilder.DropTable(
                name: "inspection_photos");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "inspection_item_categories");

            migrationBuilder.DropTable(
                name: "inspections");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "drivers");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
