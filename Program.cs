using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using fleetops_backend.Application.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using EFCore.NamingConventions;
using fleetops_backend.Infrastructure.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using fleetops_backend.Models;
using System.Threading.RateLimiting;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection("Jwt");
var keyString = jwt["Key"]
    ?? Environment.GetEnvironmentVariable("Jwt__Key")
    ?? Environment.GetEnvironmentVariable("JWT__KEY")
    ?? throw new InvalidOperationException("JWT Key is not configured. Set Jwt__Key in environment variables or appsettings.json.");
var key = Encoding.UTF8.GetBytes(keyString);
var swaggerAuth = builder.Configuration.GetSection("SwaggerAuth");
var swaggerUsername = swaggerAuth["Username"];
var swaggerPassword = swaggerAuth["Password"];

// Handle DATABASE_URL from Railway/Render
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Parse DATABASE_URL format: postgresql://user:password@host:port/database
    var uri = new Uri(databaseUrl);
    var username = uri.UserInfo.Split(':')[0];
    var password = uri.UserInfo.Split(':')[1];
    var host = uri.Host;
    var port = uri.Port;
    var database = uri.AbsolutePath.TrimStart('/');

    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseSnakeCaseNamingConvention()
        .UseNpgsql(
            connectionString,
            x => x.UseNetTopologySuite()
        ));

//Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthValidationService, AuthValidationService>();
builder.Services.AddScoped<IUsernameService, UsernameService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

//common
builder.Services.AddScoped<IImageService, CloudinaryImageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

//Admin
builder.Services.AddScoped<IAdminVehicleService, AdminVehicleService>();
builder.Services.AddScoped<IAdminVehicleMapper, AdminVehicleMapper>();
builder.Services.AddScoped<IAdminVehicleCleanupService, AdminVehicleCleanupService>();
//FleetManager
builder.Services.AddScoped<IFleetManagerAssignmentService, FleetManagerAssignmentService>();
builder.Services.AddScoped<IFleetManagerVehicleService, FleetManagerVehicleService>();
builder.Services.AddScoped<IFleetManagerStatisticsService, FleetManagerStatisticsService>();
builder.Services.AddScoped<IFleetManagerProfileService, FleetManagerProfileService>();
builder.Services.AddScoped<IAssignmentValidator, AssignmentValidator>();
builder.Services.AddScoped<IAssignmentMapper, AssignmentMapper>();
builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();
builder.Services.Configure<MaintenanceReminderOptions>(builder.Configuration.GetSection(MaintenanceReminderOptions.SectionName));
builder.Services.AddScoped<IMaintenanceValidator, MaintenanceValidator>();
builder.Services.AddScoped<IMaintenanceNotifier, MaintenanceNotifier>();
builder.Services.AddScoped<IMaintenanceReminderService, MaintenanceReminderService>();
builder.Services.AddScoped<IFleetManagerMaintenanceService, FleetManagerMaintenanceService>();
builder.Services.AddHostedService<MaintenanceReminderWorker>();
builder.Services.Configure<FuelAnomalyOptions>(builder.Configuration.GetSection(FuelAnomalyOptions.SectionName));
builder.Services.AddScoped<IFleetManagerFuelService, FleetManagerFuelService>();
builder.Services.AddScoped<IFuelBaselineCalculator, FuelBaselineCalculator>();
builder.Services.AddScoped<IFuelFillReader, FuelFillReader>();
builder.Services.AddScoped<IFuelAnomalyDetector, FuelAnomalyDetector>();
builder.Services.AddScoped<IFuelAnomalyService, FuelAnomalyService>();

builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAdminFleetManagerService, AdminFleetManagerService>();
builder.Services.AddScoped<IAdminDriverService, AdminDriverService>();
builder.Services.AddScoped<IAdminDriverValidator, AdminDriverValidator>();
builder.Services.AddScoped<IAdminDriverMapper, AdminDriverMapper>();
builder.Services.AddScoped<IAdminProfileService, AdminProfileService>();
builder.Services.AddScoped<IAdminProfileMapper, AdminProfileMapper>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

//Driver
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ITripGpsService, TripGpsService>();
builder.Services.AddScoped<IDriverInspectionService, DriverInspectionService>();
builder.Services.AddScoped<IDriverValidationService, DriverValidationService>();
builder.Services.AddScoped<IInspectionValidator, InspectionValidator>();
builder.Services.AddScoped<IInspectionMapper, InspectionMapper>();
builder.Services.AddScoped<IInspectionPhotoService, InspectionPhotoService>();
builder.Services.AddScoped<IInspectionDamageNotifier, InspectionDamageNotifier>();
builder.Services.AddScoped<IDriverDashboardService, DriverDashboardService>();
builder.Services.AddScoped<IDriverMaintenanceService, DriverMaintenanceService>();
builder.Services.AddScoped<IDriverStatisticsService, DriverStatisticsService>();
builder.Services.AddScoped<IDriverVehicleAssignmentService, DriverVehicleAssignmentService>();
builder.Services.AddScoped<IDriverProfileMapper, DriverProfileMapper>();
builder.Services.AddScoped<IDriverProfileService, DriverProfileService>();
builder.Services.AddScoped<DriverProfileService>();
builder.Services.AddScoped<IDriverFuelService, DriverFuelService>();
builder.Services.AddScoped<IFuelValidator, FuelValidator>();
builder.Services.AddScoped<IFuelMapper, FuelMapper>();
builder.Services.AddScoped<IFuelPhotoService, FuelPhotoService>();
builder.Services.AddScoped<IFuelEfficiencyCalculator, FuelEfficiencyCalculator>();
builder.Services.AddScoped<ITripValidator, TripValidator>();
builder.Services.AddScoped<ITripCalculator, TripCalculator>();
builder.Services.AddScoped<ITripMapper, TripMapper>();

//DamageReport
builder.Services.AddScoped<IDamageReportValidator, DamageReportValidator>();
builder.Services.AddScoped<ITripDamageReportNotifier, TripDamageReportNotifier>();
builder.Services.AddScoped<IDriverDamageReportService, DriverDamageReportService>();
builder.Services.AddScoped<IDamageReportService, DamageReportService>();

//TripRoute
builder.Services.AddScoped<ITripRouteService, TripRouteService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();
var gpsBatchIntervalSeconds = int.TryParse(
    Environment.GetEnvironmentVariable("GPS_BATCH_INTERVAL_SECONDS"),
    out var configuredGpsBatchIntervalSeconds)
    ? Math.Max(1, configuredGpsBatchIntervalSeconds)
    : 5;

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("gps-batch", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1,
                Window = TimeSpan.FromSeconds(gpsBatchIntervalSeconds),
                QueueLimit = 0
            }));
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Auto-run migrations on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration completed successfully");
        
        // Auto-create PostGIS extension
        dbContext.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS postgis;");
        Console.WriteLine("PostGIS extension enabled successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration/PostGIS setup failed: {ex.Message}");
        throw;
    }
}

// Enable Swagger UI
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger") && !IsSwaggerAuthorized(context, swaggerUsername, swaggerPassword))
    {
        context.Response.Headers.WWWAuthenticate = "Basic realm=\"Swagger\"";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    await next();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Map health check endpoint (no auth required)
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

static void LoadDotEnv()
{
    var searchDirectories = new[]
    {
        Directory.GetCurrentDirectory(),
        AppContext.BaseDirectory
    };

    foreach (var directory in searchDirectories)
    {
        var currentDirectory = new DirectoryInfo(directory);

        while (currentDirectory is not null)
        {
            var envFilePath = Path.Combine(currentDirectory.FullName, ".env");
            if (File.Exists(envFilePath))
            {
                foreach (var line in File.ReadAllLines(envFilePath))
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
                    {
                        continue;
                    }

                    var separatorIndex = trimmedLine.IndexOf('=');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    var key = trimmedLine[..separatorIndex].Trim();
                    var value = trimmedLine[(separatorIndex + 1)..].Trim();

                    if ((value.StartsWith('"') && value.EndsWith('"')) ||
                        (value.StartsWith('\'') && value.EndsWith('\'')))
                    {
                        value = value[1..^1];
                    }

                    if (string.IsNullOrWhiteSpace(key) || Environment.GetEnvironmentVariable(key) is not null)
                    {
                        continue;
                    }

                    Environment.SetEnvironmentVariable(key, value);
                }

                return;
            }

            currentDirectory = currentDirectory.Parent;
        }
    }
}

static bool IsSwaggerAuthorized(HttpContext context, string? expectedUsername, string? expectedPassword)
{
    if (string.IsNullOrWhiteSpace(expectedUsername) || string.IsNullOrWhiteSpace(expectedPassword))
    {
        return true;
    }

    if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
    {
        return false;
    }

    var authorizationValue = authorizationHeader.ToString();
    if (!authorizationValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    try
    {
        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationValue[6..]));
        var separatorIndex = credentials.IndexOf(':');
        if (separatorIndex < 0)
        {
            return false;
        }

        var username = credentials[..separatorIndex];
        var password = credentials[(separatorIndex + 1)..];

        return string.Equals(username, expectedUsername, StringComparison.Ordinal) &&
                string.Equals(password, expectedPassword, StringComparison.Ordinal);
    }
    catch
    {
        return false;
    }
}