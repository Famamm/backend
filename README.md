# FleetOps Backend

Backend API untuk **FleetOps** — platform manajemen operasional armada (fleet management) berbasis multi-organisasi. Dibangun dengan **ASP.NET Core (.NET 10)**, **Entity Framework Core**, dan **PostgreSQL + PostGIS**.

Aplikasi ini melayani tiga peran pengguna:

- **Admin** — mengelola organisasi, driver, fleet manager, user, dan kendaraan secara global.
- **Fleet Manager** — mengelola armada dalam satu organisasi: penugasan driver, pemeliharaan, statistik, dan pemantauan bahan bakar.
- **Driver** — menjalankan trip, mencatat GPS, inspeksi kendaraan, pengisian bahan bakar, dan laporan kerusakan.

Fitur utama: autentikasi JWT (access + refresh token), manajemen driver & kendaraan, penugasan driver↔kendaraan, trip & pelacakan GPS (PostGIS), inspeksi kendaraan berfoto, pencatatan bahan bakar + deteksi anomali, jadwal pemeliharaan dengan pengingat otomatis, laporan kerusakan bertingkat prioritas, dan sistem notifikasi in-app.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
  - [Option A — Local Development (`dotnet run`)](#option-a--local-development-dotnet-run)
  - [Option B — Docker Compose](#option-b--docker-compose)
  - [Environment Variables](#environment-variables)
  - [Verify the Setup](#verify-the-setup)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Database Schema](#database-schema)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
  - [Authentication](#authentication)
  - [Admin](#admin)
  - [Fleet Manager](#fleet-manager)
  - [Driver](#driver)
  - [Common](#common)
- [Background Workers & Rate Limiting](#background-workers--rate-limiting)
- [Deployment](#deployment)
- [Development](#development)
- [Next Steps](#next-steps)

---

## Tech Stack

| Layer | Technology |
| --- | --- |
| Runtime | .NET 10 (`net10.0`) |
| Web framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 (`Npgsql.EntityFrameworkCore.PostgreSQL`, `EFCore.NamingConventions`) |
| Database | PostgreSQL with **PostGIS** (geography/geometry columns) |
| Auth | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) + BCrypt password hashing |
| File storage | Cloudinary (`CloudinaryDotNet`) for trip, inspection, damage, and profile images |
| API docs | Swashbuckle / OpenAPI + `Swashbuckle.AspNetCore.Filters`, HTTP Basic protection for Swagger UI |
| Health checks | `AspNetCore.HealthChecks.NpgSql` (`/health`) |
| API testing | Postman collection in `.postman/` + `postman/` |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions → Docker Hub |

> Note: this project targets .NET 10 and consequently uses **EF Core 10** and Npgsql 10. The .NET 10 SDK (or newer) is required.

---

## Prerequisites

- **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** (verified with `10.0.102`)
- **PostgreSQL 12+** with the **PostGIS** extension, or Docker (the compose files run `postgis/postgis`)
- A **Cloudinary** account (cloud name, API key, API secret) — required for any feature that uploads images
- Optional: Docker Desktop (for the containerized workflow), Postman (for the provided collection)

---

## Setup

The repository supports two workflows. Both rely on the same `.env`-style configuration described in [Environment Variables](#environment-variables).

### Option A — Local Development (`dotnet run`)

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd FleetOps-backend
   ```

2. **Start PostgreSQL with PostGIS** (skip if you already have one):

   ```bash
   docker compose -f docker-compose.local.yml up -d
   ```

   This starts the database container defined in `docker-compose.local.yml` (`postgis/postgis`), exposing port `5432`.

3. **Configure the database connection** in `appsettings.Development.json` (or via the `ConnectionStrings__DefaultConnection` environment variable):

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=fleetops_db;Username=fleetops_admin;Password=change-me"
     }
   }
   ```

4. **Configure JWT and Cloudinary** — set `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, and the `Cloudinary:*` values. For local work prefer `dotnet user-secrets` or environment variables over committing secrets:

   ```bash
   dotnet user-secrets set "Jwt:Key" "<a-long-random-secret>"
   dotnet user-secrets set "Cloudinary:CloudName" "<cloud-name>"
   dotnet user-secrets set "Cloudinary:ApiKey" "<api-key>"
   dotnet user-secrets set "Cloudinary:ApiSecret" "<api-secret>"
   ```

5. **Restore, build, and run**

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

   On startup the application automatically:
   - enables the **PostGIS** extension, and
   - applies all pending EF Core migrations (`Database.Migrate()`),

   so no manual migration step is normally required (see [Database Schema](#database-schema) for the `dotnet ef` commands).

6. The API is available at the URL printed by Kestrel (defaults: `http://localhost:5144`).

### Option B — Docker Compose

1. **Create your environment file** from the template:

   ```bash
   cp .env.example .env
   ```

   Then edit `.env` to set real passwords, the JWT key, Cloudinary credentials, and Swagger credentials.

2. **Build and start the stack**:

   ```bash
   docker compose up -d --build
   ```

   `docker-compose.yml` builds the API image from the local `Dockerfile` and runs it alongside a PostGIS database. `docker-compose.local.yml` is the lightweight variant intended for local development (database only, or DB + API depending on the profile you need).

3. The API container listens on the port configured by `API_PORT` (default `5144`). Migrations and the PostGIS extension are applied automatically at container start.

4. **Tear down**:

   ```bash
   docker compose down          # stop containers
   docker compose down -v       # stop containers and remove volumes (wipes the DB)
   ```

### Environment Variables

Copy `.env.example` to `.env`. The template documents every supported variable; the important groups are:

| Group | Keys | Purpose |
| --- | --- | --- |
| PostgreSQL | `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, `DB_CONTAINER_NAME`, `DB_PORT` | Database credentials/ports used by the compose stack |
| API | `API_ENVIRONMENT`, `API_IMAGE`, `API_CONTAINER_NAME`, `API_PORT` | ASP.NET environment and container naming/ports |
| Connection string | `ConnectionStrings__DefaultConnection` | The connection string the API uses (double underscore = config section separator) |
| JWT | `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__AccessTokenExpireMinutes`, `Jwt__RefreshTokenExpireDays` | Token signing and lifetimes |
| Cloudinary | `Cloudinary__CloudName`, `Cloudinary__ApiKey`, `Cloudinary__ApiSecret` | Image uploads |
| Swagger auth | `SwaggerAuth__Username`, `SwaggerAuth__Password` | HTTP Basic credentials protecting Swagger UI |
| GPS | `GPS_BATCH_INTERVAL_SECONDS` | Client-side GPS upload cadence advertised to apps (default `5`) |
| Maintenance reminders | `MaintenanceReminder__ReminderDays__0..N`, `MaintenanceReminder__KmBefore`, `MaintenanceReminder__ScanIntervalMinutes` | Tiered H-day reminders and odometer threshold |
| Docker Hub (CI) | `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN`, `DOCKERHUB_IMAGE`, `APP_IMAGE_TAG`, `APP_NAME` | Used by `build-push.ps1` and the GitHub Actions workflow |

### Verify the Setup

| Check | Command / URL |
| --- | --- |
| Build | `dotnet build` |
| Run | `dotnet run` |
| Health | `GET http://localhost:5144/health` |
| Swagger UI | `http://localhost:5144/swagger` |
| OpenAPI JSON | `http://localhost:5144/swagger/v1/swagger.json` |
| Login smoke test | `POST http://localhost:5144/api/auth/login` with `{ "identity": "...", "password": "..." }` |

If `SwaggerAuth:Username`/`Password` are configured, the Swagger UI prompts for HTTP Basic credentials; the OpenAPI JSON endpoint is likewise protected.

---

## Project Structure

The solution follows a layered (Clean-Architecture-inspired) folder layout. `fleetops_backend.sln` contains a single project, `fleetops_backend.csproj`.

```text
FleetOps-backend/
├── Program.cs                     # Composition root: DI, auth, CORS, rate limiting, Swagger, workers, migrate-on-start
├── fleetops_backend.csproj        # .NET 10 project file + NuGet dependencies
├── fleetops_backend.sln
├── appsettings.json               # Base config (JWT, Cloudinary, reminder/anomaly options, connection string)
├── appsettings.Development.json   # Local overrides
├── appsettings.Production.json    # Production overrides
├── Dockerfile                     # Multi-stage API image
├── docker-compose.yml             # API + PostGIS stack
├── docker-compose.local.yml       # Local (DB-focused) stack
├── build-push.ps1                 # Build & push image to Docker Hub
├── dotnet-tools.json              # Local tool manifest (dotnet-ef)
├── .env.example                   # Template for all environment variables
│
├── Presentation/                  # HTTP entry points
│   └── Controllers/
│       ├── Admin/                 # AdminDashboard, AdminDriver, AdminFleetManager, AdminProfile, AdminUser, AdminVehicle
│       ├── Auth/                  # AuthController
│       ├── Common/                # DamageReport, TripRoute (Admin + Fleet Manager)
│       ├── Driver/                # Trips, Inspections, Fuel, Dashboard, Statistics, Maintenance, DamageReport, Notifications, Profile
│       └── FleetManager/          # Vehicle, Assignment, Maintenance, Fuel, Statistics, Notifications, Profile
│
├── Application/                   # Use cases / business logic
│   ├── DTOs/
│   │   ├── Admin/  Auth/  Common/  Driver/  FleetManager/  Gen/
│   └── Services/                  # One folder per feature, each with an Interfaces/ sub-folder
│       ├── Admin/                 # Dashboard, Driver, Fuel, Profile, User, Vehicle
│       ├── Auth/                  # Token (JWT issue/refresh), Username generation
│       ├── Common/                # DamageReport, Notification, TripRoute
│       ├── Driver/                # Trip, Inspection, Fuel, Dashboard, Maintenance, DamageReport, Statistics, Profile
│       └── FleetManager/          # Vehicle, Assignment, Maintenance, Fuel, Statistics, Profile
│
├── Common/                        # Cross-cutting helpers (ApiResponse<T>, PasswordGenerator)
│
├── Infrastructure/
│   └── Data/
│       ├── AppDBContext.cs        # EF Core DbContext: DbSets, relationships, PostGIS, snake_case naming
│       └── seeds/                 # Development seed data (Driver, Vehicle, User, Role, Trip, Fuel, Inspection*, Maintenance, …)
│
├── Migrations/                    # EF Core migrations (see Database Schema)
│
├── Model/                         # Domain entities & enums (see Database Schema)
│
├── fleetops_backend.Tests/        # xUnit-style unit tests (DashboardRangeWindow, FleetActivityChartBuilder)
├── Test/                          # Manual API scratch files: fleetops_backend.http, auth-refresh-token.http, inspections.http
├── docs/
│   ├── endpoint-naming.md         # Route naming conventions (source of truth for the API surface)
│   └── sql/maintenance-queries.sql# Ready-to-run PostgreSQL queries for maintenance reporting
├── .postman/                      # Postman workspace metadata
└── postman/                       # Postman collections & environments ("FleetOps API Tests")
```

---

## Architecture

| Layer | Responsibility |
| --- | --- |
| **Presentation** | Controllers only. They bind HTTP requests to DTOs, read the caller's identity/role/organization from claims, and call a service. No business logic or direct `DbContext` access. |
| **Application** | DTOs + services implementing the use cases. Services own validation, authorization scoping (organization isolation), transactions, Cloudinary uploads, and notification creation. Services expose interfaces consumed via DI. |
| **Infrastructure** | `AppDBContext`, entity configuration, seeds, and migrations (PostgreSQL/PostGIS). |
| **Model** | Plain entity classes and enums (`DamageReportStatus`, `InspectionCondition`, `MaintenanceStatus`, `NotificationType`, `DamageSeverity`, `OperationalStatus`). |

Conventions worth knowing:

- **Organization scoping** — every driver/vehicle/trip/maintenance/fuel/inspection/damage record carries an `organization_id`. Services filter by the caller's organization claim so tenants never see each other's data.
- **Roles** — `Admin`, `FleetManager`, `Driver`. Controllers are annotated with `[Authorize(Roles = ...)]`; the common damage-report and trip-route controllers additionally allow a set of roles.
- **Responses** — most endpoints return a consistent envelope: `ApiResponse<T> { Success, Message, Data }`.
- **Uploads** — any endpoint accepting images uses `multipart/form-data` and stores files on Cloudinary, persisting only the returned URL path.
- **Localization** — user-facing messages are largely in Indonesian (e.g., statuses `Scheduled/InProgress/Completed/Cancelled`, severity `Ringan/Sedang/Berat`).

---

## Database Schema

EF Core with the **snake_case naming convention** (`EFCore.NamingConventions`), so tables and columns are pluralized *snake_case* (`users`, `trip_gps_points`, `damage_reports`, …). Explicit `[Column("...")]` attributes keep the legacy Indonesian names (`kord_awal`, `odometer_akhir`, `waktu_mulai`, `plat_nomor`). Geometry/geography columns use **PostGIS geography (Point, 4326)**.

`AppDbContext` registers 20 entity sets:

| Entity | Table | Key fields | Notes |
| --- | --- | --- | --- |
| `Organization` | `organizations` | `id`, `organization_name`, `created_at` | Tenant root |
| `Role` | `roles` | `id`, `name` | Seeded: Admin, FleetManager, Driver |
| `User` | `users` | `id`, `full_name`, `username` (unique), `email`, `address`, `phone_number`, `profile_photo`, `password_hash`, `role_id`, `organization_id`, `created_at` | BCrypt-hashed password; `username` is auto-generated |
| `Driver` | `drivers` | `id`, `user_id`, `organization_id`, `jenis_lisensi`, `tanggal_berlaku`, `foto_lisensi`, `status` | 1↔1 with `User` |
| `FleetManager` | `fleet_managers` | `id`, `user_id`, `organization_id`, `status` | 1↔1 with `User` |
| `Vehicle` | `vehicles` | `id`, `driver_id` (nullable), `organization_id`, `vehicle_name`, `plat_nomor`, `tahun_pembuatan`, `odometer`, `tipe`, `status`, `muatan_max`, `tipe_bensin`, `image_url` | `tipe_bensin` enum: `Diesel=0`, `Bensin=1` |
| `Trip` | `trips` | `id`, `driver_id`, `vehicle_id`, `kord_awal`, `kord_akhir`, `lokasi_awal`, `lokasi_akhir`, `odometer_awal`, `odometer_akhir`, `waktu_mulai`, `waktu_selesai`, `catatan`, `status`, `organization_id` | `kord_*` are geography points |
| `TripDetail` | `trip_details` | `id`, `trip_id`, `jarak`, `avg_speed`, `waktu`, `image_url` | Trip summary/metrics |
| `TripGpsPoint` | `trip_gps_points` | `id`, `trip_id`, `client_id` (GUID, dedupe), `coordinate` (geography), `captured_at`, `accuracy_meters` | Batched GPS track points |
| `Fuel` | `fuels` | `id`, `vehicle_id`, `driver_id`, `organization_id`, `fuel_price_id`, `jml_liter`, `harga_total`, `lokasi_pengisian` (geography), `odometer_isi`, `image_url_path`, `catatan`, `created_at` | Refueling records |
| `FuelPrice` | `fuel_prices` | `id`, `fuel_type`, `price_per_liter`, `created_at`, `updated_at` | Reference prices (Pertalite, BioSolar, Pertamax, …) |
| `Maintenance` | `maintenances` | `id`, `vehicle_id`, `organization_id`, `jenis`, `status`, `tanggal_jadwal`, `target_odometer`, `tanggal`, `catatan`, `odometer`, `biaya`, `reminder_sent_at`, `overdue_notified_at`, `reminder_stage`, `created_at` | Tiered reminders |
| `Inspection` | `inspections` | `id`, `driver_id`, `vehicle_id`, `organization_id`, `status`, `created_at` | Header of a checklist |
| `InspectionItemCategory` | `inspection_item_categories` | `id`, `name` | Seeded |
| `InspectionItem` | `inspection_items` | `id`, `inspection_item_category_id`, `name`, `is_required` | Seeded checklist items |
| `InspectionPhoto` | `inspection_photos` | `id`, `inspection_id`, `is_required`, `image_url_path` | |
| `InspectionResult` | `inspection_results` | `id`, `inspection_id`, `item_id`, `photo_id` (nullable), `condition`, `note` | `condition` enum: `NotOk=0`, `Ok=1` |
| `DamageReport` | `damage_reports` | `id`, `trip_id`, `driver_id`, `vehicle_id`, `organization_id`, `severity`, `priority`, `operational_status`, `status`, `damage_type`, `note`, `incident_at`, `location`, `odometer`, `image_url_path`, `review_note`, `resolved_at`, `created_at` | `status`: `Reported/InReview/InRepair/Resolved/Rejected` |
| `Notification` | `notifications` | `id`, `user_id`, `type`, `title`, `message`, `reference_id`, `is_read`, `created_at` | In-app notifications |
| `RefreshToken` | `refresh_tokens` | `id`, `user_id`, `token`, `created_at`, `expires_at`, `revoked_at` | `is_valid`/`is_revoked` are computed |

### Enums

| Enum | Values |
| --- | --- |
| `TipeBensin` | `Diesel = 0`, `Bensin = 1` |
| `InspectionCondition` | `NotOk = 0`, `Ok = 1` |
| `MaintenanceStatus` | `Scheduled`, `InProgress`, `Completed`, `Cancelled` |
| `DamageReportStatus` | `Reported = 0`, `InReview = 1`, `InRepair = 2`, `Resolved = 3`, `Rejected = 4` |
| `DamageSeverity` | `Ringan = 0`, `Sedang = 1`, `Berat = 2` |
| `OperationalStatus` | `Beroperasi = 0`, `BerhentiAman = 1`, `TidakDapatBeroperasi = 2` |
| `NotificationType` | `FuelAnomaly`, `DamageReport`, `MaintenanceReminder`, `MaintenanceOverdue`, `MaintenanceStatusChanged`, `TripDamageReport`, `TripDamageReportStatusChanged` |

### Relationships (summary)

- `Organization` 1—* `User`, `Driver`, `FleetManager`, `Vehicle`, `Trip`, `Maintenance`, `Fuel`, `Inspection`, `DamageReport` (all with `DeleteBehavior.Restrict`)
- `Role` 1—* `User`; `User` 1—1 `Driver` / `FleetManager`; `User` 1—* `RefreshToken`, `Notification`
- `Driver` 1—* `Vehicle` (current assignment), `Fuel`, `Trip`, `Inspection`, `DamageReport`
- `Vehicle` 1—* `Fuel`, `Maintenance`, `Trip`, `Inspection`, `DamageReport`
- `FuelPrice` 1—* `Fuel`
- `Trip` 1—* `TripDetail`, `TripGpsPoint`, `DamageReport`
- `Inspection` 1—* `InspectionResult`, `InspectionPhoto`; `InspectionItemCategory` 1—* `InspectionItem` 1—* `InspectionResult`; `InspectionPhoto` 1—* `InspectionResult`

### Migrations

Ten migrations are checked in under `Migrations/`:

1. `20260613102901_firstMigration`
2. `20260915083902_FixBioSolarFuelTypeName`
3. `20260918032904_AddTripGpsPoints`
4. `20260918101113_AddFuelLocationGeography`
5. `20260919235450_AddNotifications`
6. `20260920010000_AddMaintenanceScheduling`
7. `20260920143700_AddMaintenanceReminderStages`
8. `20260924102335_AddDamageReports`
9. `20260925103422_AddUserPhoneNumber`
10. `20260926045122_AddFleetManagerTable`

The API applies migrations automatically on startup. To manage them manually:

```bash
# Restore the local EF tool (defined in dotnet-tools.json)
dotnet tool restore

# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# List migrations
dotnet ef migrations list

# Roll back to a previous migration
dotnet ef database update <PreviousMigrationName>
```

> Migrations target PostgreSQL with PostGIS. The application enables the `postgis` extension and registers an Npgsql health check at startup, so the database user must be allowed to `CREATE EXTENSION`.

---

## Configuration

Configuration is read from, in order of increasing precedence:

1. `appsettings.json` — shared defaults
2. `appsettings.{Environment}.json` — `Development` / `Production` overrides
3. `.env` file (loaded into process environment by the app) — see `.env.example`
4. Process environment variables / `dotnet user-secrets` in Development

ASP.NET Core maps `__` (double underscore) to nested config, e.g. `Jwt__Key` → `Jwt:Key`.

Key sections:

| Section | Keys | Notes |
| --- | --- | --- |
| `ConnectionStrings` | `DefaultConnection` | Npgsql connection string |
| `Jwt` | `Key` (**required, no default**), `Issuer` (`fleetops_backend`), `Audience` (`fleetops_backend_users`), `AccessTokenExpireMinutes` (60), `RefreshTokenExpireDays` (7) | Symmetric signing key |
| `Cloudinary` | `CloudName`, `ApiKey` (**required for photo uploads**), `ApiSecret` (**required for photo uploads**) | Image storage |
| `SwaggerAuth` | `Username`, `Password` | Optional HTTP Basic protection for Swagger UI / OpenAPI JSON |
| `MaintenanceReminder` | `ReminderDays` (int array, default `[7, 3, 1]` → H-7/H-3/H-1), `KmBefore` (500), `ScanIntervalMinutes` (1440) | Drives the maintenance reminder worker |
| `FuelAnomaly` | `Window` (5), `MinSegments` (3), `DeviationThreshold` (0.3) | Baseline-window anomaly detection |
| `GPS_BATCH_INTERVAL_SECONDS` | default `5` | Advertised GPS upload cadence |
| `Logging` | `LogLevel` | Default `Information`, `Microsoft.AspNetCore: Warning` |

> 🔐 **Secrets are never stored in tracked files.** `appsettings.json` and `appsettings.Development.json` contain placeholders only — real values come from `.env`, environment variables, or `dotnet user-secrets`:
>
> | Setting | Required? | Behaviour when missing |
> | --- | --- | --- |
> | `Jwt__Key` | Yes | The app **throws at startup** (`InvalidOperationException: JWT Key is not configured`). There is deliberately no fallback default, so a misconfigured deployment fails fast instead of signing tokens with a guessable key. |
> | `Cloudinary__ApiKey` / `Cloudinary__ApiSecret` | Only for upload endpoints | Throws `InvalidOperationException: Cloudinary settings are not configured.` on the first upload request. |
> | `ConnectionStrings__DefaultConnection` | Yes | Database connection fails on first use. `DATABASE_URL` (Railway/Render format) overrides it when set. |
>
> `.env` is gitignored (`.env.example` is the tracked template). Note that `.env` values only fill variables that are *not already set* in the environment, so real env vars always win.

---

## API Endpoints

All routes follow the conventions in [`docs/endpoint-naming.md`](docs/endpoint-naming.md). Successful responses are generally wrapped as `{ success, message, data }`. Errors return `{ message, error? }` with an appropriate status code (`400`, `401`, `403`, `404`, `500`).

Authenticated requests must include `Authorization: Bearer <accessToken>`. Role requirements are enforced per controller with `[Authorize(Roles = "...")]`.

### Authentication

Controller: `AuthController` · route prefix `api/auth` · anonymous unless noted.

| Method | Route | Body | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | `{ organizationName, fullName, email, password }` | Creates a new organization and its first **Admin** user. Returns `400` if the email is already registered. |
| `POST` | `/api/auth/login` | `{ identity, password }` | Authenticates by **username *or* email** (`identity`). Returns `{ accessToken, refreshToken, expiresIn, user }`. Returns `401` on bad credentials. |
| `POST` | `/api/auth/refresh` | `{ refreshToken }` | Exchanges a valid, non-revoked refresh token for new tokens. Returns `401` if expired/revoked/already used. |
| `POST` | `/api/auth/logout` | `{ refreshToken }` | Revokes the supplied refresh token. Returns `400` if not found. |
| `POST` | `/api/auth/logout-all` | — (requires bearer token) | Revokes **all** refresh tokens for the caller (log out everywhere). |

**Login response** (`LoginResponseDTO`):

```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<opaque token>",
  "expiresIn": 3600,
  "user": { "id": 1, "fullName": "…", "username": "…", "email": "…", "role": "Admin", "organizationId": 1 }
}
```

### Admin

Role: **Admin**. Controllers below all require `[Authorize(Roles = "Admin")]`.

#### Dashboard & Statistics — `api/admin`

| Method | Route | Query | Description |
| --- | --- | --- | --- |
| `GET` | `/api/admin/dashboard/me` | `bulan`, `tahun`, `rangeArmada`, `rangeBbm` | Aggregated admin dashboard (fleet + fuel ranges). |
| `GET` | `/api/admin/dashboard/statistics` | `bulan`, `tahun`, `rangeArmada`, `rangeBbm` | Statistics for the dashboard. |
| `GET` | `/api/admin/statistics` | `bulan`, `tahun`, `rangeArmada`, `rangeBbm` | Alias/alternative statistics route. |

#### Drivers — `api/admin/drivers`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `POST` | `/api/admin/drivers` | `multipart/form-data` | Creates a driver (and its backing `User`). Auto-generates a username; returns the initial password. |
| `GET` | `/api/admin/drivers` | — | List drivers. |
| `GET` | `/api/admin/drivers/{driverId}` | — | Driver detail. |
| `PUT` | `/api/admin/drivers/{driverId}` | `multipart/form-data` | Updates driver + account (e.g. license data, photo). Requires `TanggalBerlaku` in some update paths. |
| `DELETE` | `/api/admin/drivers/{driverId}` | — | Deletes a driver. |
| `POST` | `/api/admin/drivers/{driverId}/password/reset` | — | Resets the driver's password to a newly generated one. |

#### Fleet Managers — `api/admin/fleet-managers`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `POST` | `/api/admin/fleet-managers` | `multipart/form-data` | Creates a fleet manager account. |
| `GET` | `/api/admin/fleet-managers` | — | List fleet managers. |
| `GET` | `/api/admin/fleet-managers/{userId}` | — | Fleet manager detail (by user id). |
| `PUT` | `/api/admin/fleet-managers/{userId}` | `multipart/form-data` | Updates a fleet manager. |
| `DELETE` | `/api/admin/fleet-managers/{userId}` | — | Deletes a fleet manager. |
| `POST` | `/api/admin/fleet-managers/{userId}/password/reset` | — | Resets the fleet manager's password. |

#### Users & Vehicles — `api/admin`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `GET` | `/api/admin/users` | — | List all users. |
| `POST` | `/api/admin/vehicles` | `multipart/form-data` | Creates a vehicle (with image). |
| `GET` | `/api/admin/vehicles` | — | List vehicles. |
| `GET` | `/api/admin/vehicles/{vehicleId}` | — | Vehicle detail. |
| `PUT` | `/api/admin/vehicles/{vehicleId}` | `multipart/form-data` | Updates a vehicle. |
| `DELETE` | `/api/admin/vehicles/{vehicleId}` | — | Deletes a vehicle. |

#### Profile — `api/admin/profile`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `GET` | `/api/admin/profile/me` | — | Current admin profile. |
| `PUT` | `/api/admin/profile/me` | `multipart/form-data` | Updates the current admin's profile (incl. photo). |

### Fleet Manager

Role: **Fleet Manager**. All controllers below require `[Authorize(Roles = "FleetManager")]` and are scoped to the caller's organization.

#### Vehicles & Summary — `api/fleet-manager`

| Method | Route | Query | Description |
| --- | --- | --- | --- |
| `GET` | `/api/fleet-manager/vehicles` | — | Fleet vehicles with their current driver (`FleetVehicleDTO`). |
| `GET` | `/api/fleet-manager/summary` | — | Fleet summary: totals, active vehicles, unassigned vehicles/drivers (`FleetDashboardDTO`). |

#### Driver Assignment — `api/fleet-manager`

| Method | Route | Body | Description |
| --- | --- | --- | --- |
| `POST` | `/api/fleet-manager/assignments` | `{ vehicleId, driverId }` | Assigns a driver to a vehicle. |
| `DELETE` | `/api/fleet-manager/vehicles/{vehicleId}/driver` | — | Unassigns the current driver from a vehicle. |
| `GET` | `/api/fleet-manager/vehicles/{vehicleId}/current-driver` | — | Current driver of a vehicle (`CurrentDriverDTO`). |
| `GET` | `/api/fleet-manager/drivers/available` | — | Drivers with no current assignment (`AvailableDriverDTO`). |

#### Maintenance — `api/fleet-manager`

| Method | Route | Body | Description |
| --- | --- | --- | --- |
| `POST` | `/api/fleet-manager/vehicles/{vehicleId}/maintenances` | `{ jenis, tanggalJadwal?, targetOdometer?, catatan? }` | Schedules maintenance for a vehicle. |
| `GET` | `/api/fleet-manager/vehicles/{vehicleId}/maintenances` | — | Maintenance history for a vehicle. |
| `GET` | `/api/fleet-manager/vehicles/maintenance-status` | — | Per-vehicle maintenance status (`DueState`, active/next/last completed). |
| `GET` | `/api/fleet-manager/maintenances/{maintenanceId}` | — | Maintenance detail. |
| `PUT` | `/api/fleet-manager/maintenances/{maintenanceId}` | `{ jenis, tanggalJadwal?, targetOdometer?, catatan? }` | Updates a scheduled maintenance. |
| `DELETE` | `/api/fleet-manager/maintenances/{maintenanceId}` | — | Deletes a maintenance record. |
| `PUT` | `/api/fleet-manager/maintenances/{maintenanceId}/status` | `{ status, odometer?, biaya?, catatan? }` | Transitions status (`Scheduled`, `InProgress`, `Completed`, `Cancelled`); emits notifications. |

#### Fuel, Statistics & Notifications — `api/fleet-manager`

| Method | Route | Query | Description |
| --- | --- | --- | --- |
| `GET` | `/api/fleet-manager/vehicles/{vehicleId}/fuel-baseline` | — | Fuel efficiency baseline (average km/l, reliability) for anomaly detection. |
| `GET` | `/api/fleet-manager/statistics` | `bulan`, `tahun` | Fleet statistics: vehicle mix, trips, distance, fuel volume/cost, activity per day. |
| `GET` | `/api/fleet-manager/notifications` | `unreadOnly` (bool, default `false`) | List notifications for the caller. |
| `PATCH` | `/api/fleet-manager/notifications/{notificationId}/read` | — | Mark one notification as read. |
| `PATCH` | `/api/fleet-manager/notifications/read-all` | — | Mark all notifications as read. |

#### Profile — `api/fleet-manager/profile`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `GET` | `/api/fleet-manager/profile/me` | — | Current fleet-manager profile. |
| `PUT` | `/api/fleet-manager/profile/me` | `multipart/form-data` | Updates the current profile (`fullName`, `email`, `phoneNumber`, `profilePhoto`). |

### Driver

Role: **Driver**. All controllers below require `[Authorize(Roles = "Driver")]`.

#### Trips — `api/drivers/trips`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `POST` | `/api/drivers/trips` | `application/json` | Starts a trip. Body: `{ driverId, vehicleId, lokasiAwal?, lokasiAkhir?, kordAwalLatitude?, kordAwalLongitude?, odometerAwal? }`. |
| `PATCH` | `/api/drivers/trips/{tripId}/stop` | `multipart/form-data` | Ends a trip. Body: `{ driverId, vehicleId, lokasiAkhir?, kordAkhirLatitude?, kordAkhirLongitude?, odometerAkhir?, catatan?, image }` — `image` is required. |
| `POST` | `/api/drivers/trips/{tripId}/gps` | `application/json` | Uploads a **batch** of GPS points: `{ points: [{ clientId, latitude, longitude, capturedAt, accuracyMeters? }] }`. Deduplicated by `clientId`; returns `{ receivedCount, savedCount, duplicateCount }`. **Rate limited** (`gps-batch` policy). |
| `PATCH` | `/api/drivers/trips/{tripId}/cancel` | `application/json` | Cancels a trip. |
| `GET` | `/api/drivers/trips/driver/{driverId}` | — | Trip history for a driver. |
| `GET` | `/api/drivers/trips/driver/{driverId}/active` | — | The driver's currently active trip. |
| `GET` | `/api/drivers/trips/{tripId}` | — | Trip detail (incl. metrics/route). |

#### Damage Reports (driver) — `api/drivers/trips`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `POST` | `/api/drivers/trips/{tripId}/damage-reports` | `multipart/form-data` | Reports damage for a trip. Body: `{ severity (0–2), operationalStatus (0–2), damageType?, note?, location?, odometer?, image? }`. `priority` is derived from severity + operational status. |
| `GET` | `/api/drivers/trips/{tripId}/damage-reports` | — | Damage reports for a trip. |

#### Inspections — `api/drivers/inspections`

| Method | Route | Content type | Description |
| --- | --- | --- | --- |
| `POST` | `/api/drivers/inspections` | `multipart/form-data` | Creates an inspection. Body: `{ driverId, vehicleId, results: [{ itemId, condition (0–1), note?, photoIndex? }], photos? }`. |
| `GET` | `/api/drivers/inspections/inspection-items` | — | Master checklist (categories + items + required photos). |
| `GET` | `/api/drivers/inspections/me` | — | Inspections of the authenticated driver. |
| `GET` | `/api/drivers/inspections/me/today` | — | Today's inspection(s) for the authenticated driver. |
| `GET` | `/api/drivers/inspections/driver/{driverId}` | — | Inspections by driver id. |
| `GET` | `/api/drivers/inspections/vehicle/{vehicleId}` | — | Inspections by vehicle id. |
| `GET` | `/api/drivers/inspections/{inspectionId}` | — | Inspection detail. |
| `PUT` | `/api/drivers/inspections/{inspectionId}` | `multipart/form-data` | Updates an inspection. |
| `DELETE` | `/api/drivers/inspections/{inspectionId}` | — | Deletes an inspection. |

#### Fuel — `api/drivers/fuel`

| Method | Route | Query/body | Description |
| --- | --- | --- | --- |
| `GET` | `/api/drivers/fuel/prices` | — | **Anonymous.** Current reference fuel prices. |
| `POST` | `/api/drivers/fuel` | `multipart/form-data` | Records a refueling. Body: `{ vehicleId, fuelType, jmlLiter, hargaTotal, odometerIsi, catatan?, fuelPhoto?, latitude?, longitude?, accuracyMeters? }`. |
| `GET` | `/api/drivers/fuel/efficiency` | — | Fuel efficiency (km/l) for the driver's vehicle. |
| `GET` | `/api/drivers/fuel/history` | — | Refueling history. |
| `GET` | `/api/drivers/fuel/{fuelId}` | — | Refueling detail. |

#### Dashboard, Statistics, Profile, Maintenance Status — `api/drivers`

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/drivers/dashboard/me` | Driver dashboard (active trip, today's inspection, metrics). |
| `GET` | `/api/drivers/statistics/me` | Driver statistics. |
| `GET` | `/api/drivers/profile/me` | Driver profile (incl. license info). |
| `GET` | `/api/drivers/vehicles/me/maintenance-status` | Maintenance status of the vehicle currently assigned to the driver. |

#### Notifications — `api/drivers/notifications`

| Method | Route | Query | Description |
| --- | --- | --- | --- |
| `GET` | `/api/drivers/notifications` | `unreadOnly` (bool, default `false`) | List notifications for the driver. |
| `PATCH` | `/api/drivers/notifications/{notificationId}/read` | — | Mark one notification as read. |
| `PATCH` | `/api/drivers/notifications/read-all` | — | Mark all notifications as read. |

### Common

Shared by **Admin** and **Fleet Manager** — `[Authorize(Roles = "Admin,FleetManager")]`.

#### Damage Reports — `api/damage-reports`

| Method | Route | Body | Description |
| --- | --- | --- | --- |
| `GET` | `/api/damage-reports` | query `status?` | Lists damage reports, optionally filtered by status. |
| `GET` | `/api/damage-reports/{id}` | — | Damage report detail. |
| `PATCH` | `/api/damage-reports/{id}/status` | `{ status, reviewNote? }` | Updates the report status (`Reported`, `InReview`, `InRepair`, `Resolved`, `Rejected`). |

#### Trip Routes — `api/trips`

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/trips/live-locations` | Latest GPS location of active trips in the organization. |
| `GET` | `/api/trips/{tripId}/gps` | Full GPS route of a trip. |

#### Misc

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `GET` | `/health` | Anonymous | Health check endpoint (Npgsql probe). |
| `GET` | `/swagger` | Basic auth (if configured) | Swagger UI. |
| `GET` | `/swagger/v1/swagger.json` | Basic auth (if configured) | OpenAPI document. |

---

## Background Workers & Rate Limiting

- **`MaintenanceReminderWorker`** — a hosted service (`AddHostedService`) that scans maintenance schedules on an interval (`MaintenanceReminder__ScanIntervalMinutes`, default daily) and emits tiered reminders (`ReminderDays`, default H-7/H-3/H-1) plus the odometer threshold (`KmBefore`). Notification types produced: `MaintenanceReminder`, `MaintenanceOverdue`, and `MaintenanceStatusChanged` (on status transitions).
- **GPS batch rate limiting** — a fixed-window rate limiter named `gps-batch` protects `POST /api/drivers/trips/{tripId}/gps`. The window equals `GPS_BATCH_INTERVAL_SECONDS` (default 5s, minimum 1s), matching the client upload cadence.
- **Fuel anomaly detection** — computed from a rolling baseline window (`FuelAnomaly:Window`, `MinSegments`, `DeviationThreshold`); confirmed anomalies raise `FuelAnomaly` notifications.
- **CORS** — a policy named `AllowFrontend` is configured in `Program.cs`.
- **Background reference queries** — `docs/sql/maintenance-queries.sql` contains standalone PostgreSQL queries (using a `params` CTE) for maintenance reporting and due-state calculations.

---

## Deployment

### Docker image

`Dockerfile` is a three-stage build (`sdk:10.0` → publish → `aspnet:10.0`):

- `ASPNETCORE_URLS` / `EXPOSE` follow the `API_PORT` build arg (default `5144`).
- `ASPNETCORE_ENVIRONMENT` follows the `API_ENVIRONMENT` build arg (default `Production`).
- A `HEALTHCHECK` calls `curl -f http://localhost:${API_PORT}/health`.

```bash
docker build -t fleetopsapp/fleetops-api:dev .
docker run --env-file .env -p 5144:5144 fleetopsapp/fleetops-api:dev
```

### Docker Compose

| File | Purpose |
| --- | --- |
| `docker-compose.yml` | Full stack: API image + PostGIS database. |
| `docker-compose.local.yml` | Local-development stack (database-focused). |

### CI — GitHub Actions

`.github/workflows/docker-build.yml` (**Docker Build**) runs on `push` and `pull_request` to `develop` and `main`:

- Image name: `vars.DOCKERHUB_IMAGE` (fallback `fleetopsapp/fleetops-api`).
- Tag: `production` on `main`, otherwise `dev`; PR builds are tagged `pr-<number>` and are **not** pushed.
- Credentials: repository secrets `DOCKERHUB_USERNAME` / `DOCKERHUB_TOKEN`.
- Uses `docker/setup-buildx-action`, `docker/login-action`, and `docker/build-push-action`.

### Manual build & push

`build-push.ps1` mirrors the workflow for local/CI-less usage. It reads the same `.env` file (so the DB/JWT/Cloudinary values match compose) and supports:

```powershell
# Production tag (simulating main)
./build-push.ps1 -Branch main

# PR mode: build only, tag pr-42
./build-push.ps1 -PrNumber 42

# Extra options
./build-push.ps1 -EnvFile ./.env -NoCache -SkipLogin
```

---

## Development

### API testing

- **Postman** — import the collection from `postman/collections/` (`FleetOps API Tests`), which contains request/environment definitions for the whole API. Workspace metadata lives in `.postman/`.
- **`.http` scratch files** — `Test/fleetops_backend.http`, `Test/auth-refresh-token.http`, and `Test/inspections.http` can be executed from VS Code's REST Client.
- **Swagger UI** — live OpenAPI docs at `/swagger` once the app is running.

### Endpoint naming

Follow [`docs/endpoint-naming.md`](docs/endpoint-naming.md) when adding routes. In short: plural `kebab-case` resource segments (`/api/admin/fleet-managers`), actions attached to the resource (`POST /api/drivers/trips`, `PATCH /api/drivers/trips/{tripId}/stop`), and role-based prefixes (`/api/admin`, `/api/fleet-manager`, `/api/drivers`).

### Unit tests

`fleetops_backend.Tests/` holds xUnit-style unit tests for pure helpers (`DashboardRangeWindowTests`, `FleetActivityChartBuilderTests`). They are currently standalone source files (excluded from the main project); copy them into a dedicated test project if you want `dotnet test` to pick them up.

### Seed data & database naming

- Development seeds live in `Infrastructure/Data/seeds/` and are registered in `AppDbContext.OnModelCreating` (roles, organizations, users, drivers, vehicles, fuels, maintenance, trips, inspections, etc.).
- Table/column naming uses the snake_case convention; keep an explicit `[Column("...")]` when a legacy Indonesian name must be preserved.

### Common pitfalls

- The database **must** have PostGIS available; migrations create geography columns.
- Any image-upload flow requires valid Cloudinary credentials — missing config causes runtime errors, not compile errors.
- `username` is unique across all users and generated automatically; login accepts either username or email.

---

## Next Steps

- Add a dedicated test project (xUnit) so existing tests run via `dotnet test`.
- Introduce refresh-token rotation/cleanup and stricter rate limits for auth endpoints.
- **Rotate** the credentials that were previously committed in `appsettings.Development.json` (database password and Cloudinary API key/secret). They have been removed from the files, but they remain in **git history** — rotation is mandatory, and consider purging history with `git filter-repo` or BFG.
- Add the Postman/seeded user passwords to the rotation/cleanup list if those accounts are ever used outside local development.
- Expand CI to run `dotnet build` and `dotnet test` before the Docker publish step.

