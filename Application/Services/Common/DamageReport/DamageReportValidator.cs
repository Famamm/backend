using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class DamageReportValidator : IDamageReportValidator
    {
        private const int MaxDamageTypeLength = 100;
        private const int MaxNoteLength = 500;
        private const int MaxLocationLength = 255;

        private static readonly Dictionary<DamageReportStatus, DamageReportStatus[]> AllowedTransitions = new()
        {
            [DamageReportStatus.Reported] = [DamageReportStatus.InReview, DamageReportStatus.Rejected],
            [DamageReportStatus.InReview] = [DamageReportStatus.InRepair, DamageReportStatus.Rejected],
            [DamageReportStatus.InRepair] = [DamageReportStatus.Resolved],
            [DamageReportStatus.Resolved] = [],
            [DamageReportStatus.Rejected] = []
        };

        public DamageSeverity ParseSeverity(int value)
        {
            if (!Enum.IsDefined(typeof(DamageSeverity), value))
            {
                throw new InvalidOperationException(
                    $"Severity tidak valid. Gunakan salah satu dari: {string.Join(", ", Enum.GetNames<DamageSeverity>())}.");
            }

            return (DamageSeverity)value;
        }

        public OperationalStatus ParseOperationalStatus(int value)
        {
            if (!Enum.IsDefined(typeof(OperationalStatus), value))
            {
                throw new InvalidOperationException(
                    $"Status operasional tidak valid. Gunakan salah satu dari: {string.Join(", ", Enum.GetNames<OperationalStatus>())}.");
            }

            return (OperationalStatus)value;
        }

        public DamageReportStatus ParseStatus(string? value)
        {
            if (!Enum.TryParse<DamageReportStatus>(value, ignoreCase: true, out var status)
                || !Enum.IsDefined(status))
            {
                throw new InvalidOperationException(
                    $"Status tidak valid. Gunakan salah satu dari: {string.Join(", ", Enum.GetNames<DamageReportStatus>())}.");
            }

            return status;
        }

        public void ValidateCreate(CreateDamageReportDTO dto)
        {
            if (dto.DamageType?.Trim().Length > MaxDamageTypeLength)
            {
                throw new InvalidOperationException($"Jenis kerusakan maksimal {MaxDamageTypeLength} karakter.");
            }

            if (dto.Note?.Trim().Length > MaxNoteLength)
            {
                throw new InvalidOperationException($"Catatan maksimal {MaxNoteLength} karakter.");
            }

            if (dto.Location?.Trim().Length > MaxLocationLength)
            {
                throw new InvalidOperationException($"Lokasi maksimal {MaxLocationLength} karakter.");
            }

            if (dto.Odometer is < 0)
            {
                throw new InvalidOperationException("Odometer tidak boleh negatif.");
            }
        }

        public void ValidateTransition(DamageReportStatus from, DamageReportStatus to)
        {
            if (!AllowedTransitions[from].Contains(to))
            {
                throw new InvalidOperationException($"Status tidak dapat diubah dari {from} ke {to}.");
            }
        }

        public int DerivePriority(DamageSeverity severity, OperationalStatus operationalStatus)
        {
            if (operationalStatus == OperationalStatus.TidakDapatBeroperasi || severity == DamageSeverity.Berat)
            {
                return 1;
            }

            if (severity == DamageSeverity.Sedang || operationalStatus == OperationalStatus.BerhentiAman)
            {
                return 2;
            }

            return 3;
        }
    }
}
