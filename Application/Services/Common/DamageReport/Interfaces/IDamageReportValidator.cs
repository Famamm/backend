using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IDamageReportValidator
    {
        DamageSeverity ParseSeverity(int value);
        OperationalStatus ParseOperationalStatus(int value);
        DamageReportStatus ParseStatus(string? value);
        void ValidateCreate(CreateDamageReportDTO dto);
        void ValidateTransition(DamageReportStatus from, DamageReportStatus to);
        int DerivePriority(DamageSeverity severity, OperationalStatus operationalStatus);
    }
}
