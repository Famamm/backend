using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.Services
{
    public class DriverInspectionService : IDriverInspectionService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInspectionValidator _validator;
        private readonly IInspectionMapper _mapper;
        private readonly IInspectionPhotoService _photoService;
        private readonly IInspectionDamageNotifier _damageReportNotifier;

        public DriverInspectionService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IInspectionValidator validator,
            IInspectionMapper mapper,
            IInspectionPhotoService photoService,
            IInspectionDamageNotifier damageReportNotifier)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
            _mapper = mapper;
            _photoService = photoService;
            _damageReportNotifier = damageReportNotifier;
        }

        public async Task<GetInspectionDTO?> CreateInspectionAsync(CreateInspectionDTO dto)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.Id == dto.DriverId);
            _validator.ValidateDriverExists(driver);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId);
            _validator.ValidateVehicleExists(vehicle);

            _validator.ValidateSameOrganization(driver!, vehicle!);

            var organizationId = driver!.OrganizationId;

            var orgClaimId = GetOrganizationIdFromClaims();
            _validator.ValidateOrganizationClaim(organizationId, orgClaimId);

            var (startUtc, endUtc) = _validator.GetTodayRangeUtc(DateTimeOffset.UtcNow);

            var existingInspection = await _context.Set<Inspection>()
                .FirstOrDefaultAsync(i => i.VehicleId == dto.VehicleId && i.DriverId == dto.DriverId && i.CreatedAt >= startUtc && i.CreatedAt < endUtc);
            _validator.ValidateNoInspectionToday(existingInspection);

            var inspection = new Inspection
            {
                OrganizationId = organizationId,
                DriverId = dto.DriverId,
                VehicleId = dto.VehicleId,
                Status = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Set<Inspection>().Add(inspection);
            await _context.SaveChangesAsync();

            var photoUrls = await _photoService.UploadPhotosAsync(inspection.Id, dto.Photos);

            if (dto.Results != null && dto.Results.Count > 0)
            {
                foreach (var resultInput in dto.Results)
                {
                    var item = await _context.Set<InspectionItem>()
                        .FirstOrDefaultAsync(i => i.Id == resultInput.ItemId);
                    if (item == null)
                    {
                        throw new InvalidOperationException($"Inspection item {resultInput.ItemId} not found.");
                    }

                    int? photoId = null;
                    if (resultInput.PhotoIndex.HasValue && resultInput.PhotoIndex.Value < photoUrls.Count)
                    {
                        photoId = await _photoService.FindPhotoIdByUrlAsync(photoUrls[resultInput.PhotoIndex.Value]);
                    }

                    var inspectionResult = new InspectionResult
                    {
                        InspectionId = inspection.Id,
                        ItemId = resultInput.ItemId,
                        PhotoId = photoId,
                        Condition = (InspectionCondition)resultInput.Condition,
                        Note = resultInput.Note
                    };

                    _context.Set<InspectionResult>().Add(inspectionResult);
                }

                await _context.SaveChangesAsync();
            }

            var response = await _context.Set<Inspection>()
                .Include(i => i.Results)
                    .ThenInclude(r => r.Item)
                .Include(i => i.Vehicle)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == inspection.Id);

            if (response == null)
            {
                return null;
            }

            await _damageReportNotifier.NotifyNewDamageAsync(response, []);

            return _mapper.ToInspectionDto(response);
        }

        public async Task<List<GetInspectionDTO>> GetInspectionsByDriverAsync(int driverId)
        {
            var orgClaimId = GetOrganizationIdFromClaims();

            var inspectionsQuery = _context.Set<Inspection>().AsQueryable();
            inspectionsQuery = inspectionsQuery.Where(i => i.DriverId == driverId);
            if (orgClaimId.HasValue)
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.OrganizationId == orgClaimId.Value);
            }

            var inspections = await inspectionsQuery
                .Include(i => i.Results)
                    .ThenInclude(r => r.Item)
                .Include(i => i.Vehicle)
                .Include(i => i.Photos)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return inspections.Select(_mapper.ToInspectionDto).ToList();
        }

        public async Task<List<GetInspectionDTO>> GetInspectionsByUserIdAsync(int userId)
        {
            var driverId = await GetDriverIdByUserIdAsync(userId);
            if (driverId == null)
            {
                return new List<GetInspectionDTO>();
            }

            return await GetInspectionsByDriverAsync(driverId.Value);
        }

        public async Task<List<GetInspectionDTO>> GetTodayInspectionsByUserIdAsync(int userId)
        {
            var driverId = await GetDriverIdByUserIdAsync(userId);
            if (driverId == null)
            {
                return new List<GetInspectionDTO>();
            }

            var orgClaimId = GetOrganizationIdFromClaims();
            var (startUtc, endUtc) = _validator.GetTodayRangeUtc(DateTimeOffset.UtcNow);

            var inspectionsQuery = _context.Set<Inspection>().AsQueryable();
            inspectionsQuery = inspectionsQuery.Where(i =>
                i.DriverId == driverId.Value &&
                i.CreatedAt >= startUtc &&
                i.CreatedAt < endUtc);
            if (orgClaimId.HasValue)
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.OrganizationId == orgClaimId.Value);
            }

            var inspections = await inspectionsQuery
                .Include(i => i.Results)
                    .ThenInclude(r => r.Item)
                .Include(i => i.Vehicle)
                .Include(i => i.Photos)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return inspections.Select(_mapper.ToInspectionDto).ToList();
        }

        public async Task<List<GetInspectionDTO>> GetInspectionsByVehicleAsync(int vehicleId)
        {
            var orgClaimId = GetOrganizationIdFromClaims();

            var inspectionsQuery = _context.Set<Inspection>().AsQueryable();
            inspectionsQuery = inspectionsQuery.Where(i => i.VehicleId == vehicleId);
            if (orgClaimId.HasValue)
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.OrganizationId == orgClaimId.Value);
            }

            var inspections = await inspectionsQuery
                .Include(i => i.Results)
                    .ThenInclude(r => r.Item)
                .Include(i => i.Photos)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return inspections.Select(_mapper.ToInspectionDto).ToList();
        }

        public async Task<GetInspectionDTO?> UpdateInspectionAsync(int id, UpdateInspectionDTO dto)
        {
            var inspection = await _context.Set<Inspection>()
                .Include(i => i.Results)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspection == null)
            {
                throw new KeyNotFoundException("Inspection not found.");
            }

            var orgClaimId = GetOrganizationIdFromClaims();
            _validator.ValidateOrganizationClaim(inspection.OrganizationId, orgClaimId);

            _validator.ValidateDriverNotChanged(inspection.DriverId, dto.DriverId);

            var previousNotOkItemIds = InspectionDamageDetector.GetNotOkItemIds(inspection.Results);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId);
            _validator.ValidateVehicleExists(vehicle);

            _validator.ValidateVehicleSameOrganizationAsInspection(vehicle!, inspection.OrganizationId);

            inspection.VehicleId = dto.VehicleId;

            var photoUrls = new List<string>();

            if (dto.Photos != null && dto.Photos.Count > 0)
            {
                await _photoService.DeleteOldPhotosAsync(inspection.Photos);

                photoUrls = await _photoService.UploadPhotosAsync(inspection.Id, dto.Photos);
            }

            if (dto.Results != null && dto.Results.Count > 0)
            {
                var oldResults = await _context.Set<InspectionResult>()
                    .Where(r => r.InspectionId == id)
                    .ToListAsync();
                _context.Set<InspectionResult>().RemoveRange(oldResults);

                foreach (var resultInput in dto.Results)
                {
                    var item = await _context.Set<InspectionItem>()
                        .FirstOrDefaultAsync(i => i.Id == resultInput.ItemId);
                    if (item == null)
                    {
                        throw new InvalidOperationException($"Inspection item {resultInput.ItemId} not found.");
                    }

                    int? photoId = null;
                    if (resultInput.PhotoIndex.HasValue && resultInput.PhotoIndex.Value < photoUrls.Count)
                    {
                        photoId = await _photoService.FindPhotoIdByUrlAsync(photoUrls[resultInput.PhotoIndex.Value]);
                    }

                    var inspectionResult = new InspectionResult
                    {
                        InspectionId = inspection.Id,
                        ItemId = resultInput.ItemId,
                        PhotoId = photoId,
                        Condition = (InspectionCondition)resultInput.Condition,
                        Note = resultInput.Note
                    };

                    _context.Set<InspectionResult>().Add(inspectionResult);
                }
            }

            _context.Set<Inspection>().Update(inspection);
            await _context.SaveChangesAsync();

            var response = await _context.Set<Inspection>()
                .Include(i => i.Results)
                    .ThenInclude(r => r.Item)
                .Include(i => i.Vehicle)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == inspection.Id);

            if (response == null)
            {
                return null;
            }

            await _damageReportNotifier.NotifyNewDamageAsync(response, previousNotOkItemIds);

            return _mapper.ToInspectionDto(response);
        }

        public async Task<bool> DeleteInspectionAsync(int id)
        {
            var inspection = await _context.Set<Inspection>()
                .Include(i => i.Results)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspection == null)
            {
                return false;
            }

            var orgClaimId = GetOrganizationIdFromClaims();
            if (orgClaimId.HasValue && inspection.OrganizationId != orgClaimId.Value)
            {
                return false;
            }

            _context.Set<InspectionResult>().RemoveRange(inspection.Results);

            await _photoService.DeletePhotosAsync(inspection.Photos);

            _context.Set<Inspection>().Remove(inspection);

            await _context.SaveChangesAsync();
            return true;
        }

        // Detail inspection
        public async Task<GetDetailInspectionDTO?> GetInspectionDetailByIdAsync(int id)
        {
            var inspection = await _context.Set<Inspection>()
                .Include(i => i.Results)
                    .ThenInclude(r => r.Item)
                .Include(i => i.Vehicle)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspection == null)
            {
                return null;
            }

            var orgClaimId = GetOrganizationIdFromClaims();
            if (orgClaimId.HasValue && inspection.OrganizationId != orgClaimId.Value)
            {
                return null;
            }

            return _mapper.ToDetailInspectionDto(inspection);
        }

        // Method to get all inspection items
        public async Task<List<InspectionItemDTO>> GetInspectionItemsAsync()
        {
            var items = await _context.Set<InspectionItem>()
                .Include(i => i.Category)
                .ToListAsync();

            return items.Select(item => new InspectionItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                IsRequired = item.IsRequired,
                CategoryId = item.InspectionItemCategoryId,
                CategoryName = item.Category?.Name ?? string.Empty
            }).ToList();
        }

        // Cari driver berdasarkan user id dari token (untuk endpoint /me)
        private async Task<int?> GetDriverIdByUserIdAsync(int userId)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);

            return driver?.Id;
        }

        // Method to get organization ID from JWT claims
        private int? GetOrganizationIdFromClaims()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            return user == null ? null : ClaimsHelper.GetOrganizationIdOrNull(user);
        }
    }
}
