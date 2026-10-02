using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class DamageReportService : IDamageReportService
    {
        private readonly AppDbContext _context;
        private readonly IDamageReportValidator _validator;
        private readonly ITripDamageReportNotifier _notifier;

        public DamageReportService(
            AppDbContext context,
            IDamageReportValidator validator,
            ITripDamageReportNotifier notifier)
        {
            _context = context;
            _validator = validator;
            _notifier = notifier;
        }

        public async Task<List<DamageReportDTO>> GetAllAsync(int organizationId, string? status)
        {
            var query = _context.DamageReports
                .Include(r => r.Vehicle)
                .Include(r => r.Driver)
                    .ThenInclude(d => d!.User)
                .Where(r => r.OrganizationId == organizationId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var parsed = _validator.ParseStatus(status);
                query = query.Where(r => r.Status == parsed);
            }

            var reports = await query
                .OrderBy(r => r.Priority)
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reports.Select(DamageReportMapper.ToDto).ToList();
        }

        public async Task<DamageReportDTO> GetByIdAsync(int organizationId, int id)
        {
            var report = await FindReportAsync(organizationId, id);

            return DamageReportMapper.ToDto(report);
        }

        public async Task<DamageReportDTO> UpdateStatusAsync(
            int organizationId,
            int id,
            UpdateDamageReportStatusDTO dto)
        {
            var report = await FindReportAsync(organizationId, id);

            var newStatus = _validator.ParseStatus(dto.Status);
            _validator.ValidateTransition(report.Status, newStatus);

            report.Status = newStatus;

            if (!string.IsNullOrWhiteSpace(dto.ReviewNote))
            {
                report.ReviewNote = dto.ReviewNote.Trim();
            }

            report.ResolvedAt = newStatus == DamageReportStatus.Resolved
                ? DateTimeOffset.UtcNow
                : null;

            await _context.SaveChangesAsync();
            await _notifier.NotifyStatusChangedAsync(report);

            return DamageReportMapper.ToDto(report);
        }

        private async Task<DamageReport> FindReportAsync(int organizationId, int id)
        {
            var report = await _context.DamageReports
                .Include(r => r.Vehicle)
                .Include(r => r.Driver)
                    .ThenInclude(d => d!.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.OrganizationId == organizationId);

            if (report == null)
            {
                throw new KeyNotFoundException("Laporan kerusakan tidak ditemukan.");
            }

            return report;
        }
    }
}
