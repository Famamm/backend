using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IFuelMapper
    {
        TripFuelRecordDTO ToFuelRecordDto(Fuel fuel);
        TripFuelRecordDetailDTO ToFuelRecordDetailDto(Fuel fuel);
    }
}

