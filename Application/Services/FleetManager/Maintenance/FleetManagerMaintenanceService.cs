using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class FleetManagerMaintenanceService : IFleetManagerMaintenanceService
    {
        private const string ActiveVehicleStatus = "Active";
        private const string MaintenanceVehicleStatus = "Maintenance";
        private const string OngoingTripStatus = "Ongoing";

        private readonly AppDbContext _context;
        private readonly IMaintenanceValidator _validator;
        private readonly IMaintenanceNotifier _notifier;
        private readonly MaintenanceReminderOptions _options;

        public FleetManagerMaintenanceService(
            AppDbContext context,
            IMaintenanceValidator validator,
            IMaintenanceNotifier notifier,
            IOptions<MaintenanceReminderOptions> options)
        {
            _context = context;
            _validator = validator;
            _notifier = notifier;
            _options = options.Value;
        }

        public async Task<MaintenanceDTO> CreateAsync(int organizationId, int vehicleId, MaintenanceScheduleDTO dto)
        {
            var vehicle = await FindVehicleAsync(organizationId, vehicleId);

            _validator.ValidateVehicleSchedulable(vehicle);
            _validator.ValidateSchedule(dto, vehicle.Odometer, Today());

            var maintenance = new Maintenance
            {
                VehicleId = vehicle.Id,
                OrganizationId = organizationId,
                Jenis = dto.Jenis.Trim(),
                TanggalJadwal = dto.TanggalJadwal,
                TargetOdometer = dto.TargetOdometer,
                Catatan = dto.Catatan
            };

            _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync();

            return ToDto(maintenance, vehicle);
        }

        public async Task<List<MaintenanceDTO>> GetByVehicleAsync(int organizationId, int vehicleId)
        {
            var vehicle = await FindVehicleAsync(organizationId, vehicleId);

            var maintenances = await _context.Maintenances
                .Where(m => m.VehicleId == vehicle.Id && m.OrganizationId == organizationId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return maintenances.Select(m => ToDto(m, vehicle)).ToList();
        }

        public async Task<MaintenanceDTO> GetByIdAsync(int organizationId, int maintenanceId)
        {
            var maintenance = await FindMaintenanceAsync(organizationId, maintenanceId);

            return ToDto(maintenance, maintenance.Vehicle!);
        }

        public async Task<MaintenanceDTO> UpdateAsync(int organizationId, int maintenanceId, MaintenanceScheduleDTO dto)
        {
            var maintenance = await FindMaintenanceAsync(organizationId, maintenanceId);

            _validator.ValidateEditable(maintenance);
            _validator.ValidateSchedule(dto, maintenance.Vehicle!.Odometer, Today());

            maintenance.Jenis = dto.Jenis.Trim();
            maintenance.TanggalJadwal = dto.TanggalJadwal;
            maintenance.TargetOdometer = dto.TargetOdometer;
            maintenance.Catatan = dto.Catatan;
            maintenance.ReminderSentAt = null;
            maintenance.OverdueNotifiedAt = null;

            await _context.SaveChangesAsync();

            return ToDto(maintenance, maintenance.Vehicle!);
        }

        public async Task DeleteAsync(int organizationId, int maintenanceId)
        {
            var maintenance = await FindMaintenanceAsync(organizationId, maintenanceId);

            _validator.ValidateEditable(maintenance);

            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
        }

        public async Task<MaintenanceDTO> UpdateStatusAsync(
            int organizationId,
            int maintenanceId,
            UpdateMaintenanceStatusDTO dto)
        {
            var maintenance = await FindMaintenanceAsync(organizationId, maintenanceId);
            var vehicle = maintenance.Vehicle!;

            var newStatus = _validator.ParseStatus(dto.Status);
            _validator.ValidateTransition(maintenance.Status, newStatus);

            if (newStatus == MaintenanceStatus.InProgress)
            {
                await StartAsync(vehicle);
            }
            else if (newStatus == MaintenanceStatus.Completed)
            {
                _validator.ValidateCompletion(dto);
                Complete(maintenance, dto);
            }

            maintenance.Status = newStatus;

            if (newStatus is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            {
                await ReleaseVehicleAsync(vehicle, maintenance.Id);
            }

            await _context.SaveChangesAsync();
            await _notifier.NotifyStatusChangedAsync(maintenance);

            return ToDto(maintenance, vehicle);
        }

        public async Task<List<VehicleMaintenanceStatusDTO>> GetFleetStatusAsync(int organizationId)
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.OrganizationId == organizationId)
                .OrderBy(v => v.VehicleName)
                .Select(v => new
                {
                    Vehicle = v,
                    LastCompleted = _context.Maintenances
                        .Where(m => m.VehicleId == v.Id && m.Status == MaintenanceStatus.Completed)
                        .OrderByDescending(m => m.Tanggal)
                        .ThenByDescending(m => m.Id)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var openByVehicle = (await _context.Maintenances
                    .Where(m => m.OrganizationId == organizationId
                        && (m.Status == MaintenanceStatus.Scheduled || m.Status == MaintenanceStatus.InProgress))
                    .ToListAsync())
                .ToLookup(m => m.VehicleId);

            var today = Today();

            return vehicles
                .Select(row => MaintenanceMapper.ToVehicleStatus(
                    row.Vehicle,
                    openByVehicle[row.Vehicle.Id].ToList(),
                    row.LastCompleted,
                    today,
                    _options))
                .ToList();
        }

        private async Task StartAsync(Vehicle vehicle)
        {
            var hasOngoingTrip = await _context.Trips
                .AnyAsync(t => t.VehicleId == vehicle.Id && t.Status == OngoingTripStatus);

            if (hasOngoingTrip)
            {
                throw new InvalidOperationException("Kendaraan sedang dalam perjalanan. Tunggu trip selesai sebelum memulai pemeliharaan.");
            }

            if (vehicle.Status == ActiveVehicleStatus)
            {
                vehicle.Status = MaintenanceVehicleStatus;
            }
            else if (vehicle.Status != MaintenanceVehicleStatus)
            {
                throw new InvalidOperationException("Kendaraan tidak berstatus Active.");
            }
        }

        private static void Complete(Maintenance maintenance, UpdateMaintenanceStatusDTO dto)
        {
            maintenance.Tanggal = DateTime.UtcNow;
            maintenance.Odometer = dto.Odometer;
            maintenance.Biaya = dto.Biaya;

            if (!string.IsNullOrWhiteSpace(dto.Catatan))
            {
                maintenance.Catatan = dto.Catatan;
            }
        }

        // Kendaraan kembali Active hanya jika tidak ada pemeliharaan lain yang masih InProgress.
        private async Task ReleaseVehicleAsync(Vehicle vehicle, int finishedMaintenanceId)
        {
            if (vehicle.Status != MaintenanceVehicleStatus)
            {
                return;
            }

            var hasOtherInProgress = await _context.Maintenances
                .AnyAsync(m => m.VehicleId == vehicle.Id
                    && m.Id != finishedMaintenanceId
                    && m.Status == MaintenanceStatus.InProgress);

            if (!hasOtherInProgress)
            {
                vehicle.Status = ActiveVehicleStatus;
            }
        }

        private async Task<Vehicle> FindVehicleAsync(int organizationId, int vehicleId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.OrganizationId == organizationId)
                ?? throw new KeyNotFoundException("Kendaraan tidak ditemukan.");
        }

        private async Task<Maintenance> FindMaintenanceAsync(int organizationId, int maintenanceId)
        {
            return await _context.Maintenances
                .Include(m => m.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == maintenanceId && m.OrganizationId == organizationId)
                ?? throw new KeyNotFoundException("Jadwal pemeliharaan tidak ditemukan.");
        }

        private MaintenanceDTO ToDto(Maintenance maintenance, Vehicle vehicle)
        {
            return MaintenanceMapper.ToDto(maintenance, vehicle, Today(), _options);
        }

        private static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
