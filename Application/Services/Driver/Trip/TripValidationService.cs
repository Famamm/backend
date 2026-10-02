using fleetops_backend.Models;
using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public class TripValidator : ITripValidator
    {
        public void ValidateDriverForTrip(
            int authenticatedDriverId,
            int requestedDriverId)
        {
            if (authenticatedDriverId != requestedDriverId)
            {
                throw new InvalidOperationException(
                    "Driver does not match the authenticated user.");
            }
        }
        public void ValidateNoActiveTrip(Trip? activeTrip)
        {
            if (activeTrip != null)
            {
                throw new InvalidOperationException(
                    "Driver already has an active trip.");
            }
        }

        public void ValidateVehicleForTrip(
            Vehicle vehicle,
            int driverId,
            int? organizationId
            )
        {
            if(vehicle.DriverId != driverId)
            {
                throw new InvalidOperationException(
                    "Vehicle does not belong to the selected driver.");
            }
            if (vehicle.OrganizationId != organizationId)
            {
                throw new InvalidOperationException(
                    "Vehicle does not belong to the same organization as driver."
                );
            }
            if (vehicle.Status == "Maintenance")
            {
                throw new InvalidOperationException(
                    "Vehicle is under maintenance."
                );
            }
        }

        public void ValidateStopTrip(
            Trip trip,
            Driver driver,
            Vehicle vehicle,
            StopTripDTO dto)
        {
            if (trip.OrganizationId != driver.OrganizationId)
            {
                throw new InvalidOperationException(
                    "Driver and trip belong to different organizations.");
            }

            if (trip.DriverId != driver.Id)
            {
                throw new InvalidOperationException(
                    "Driver does not match the trip.");
            }

            if (trip.VehicleId != dto.VehicleId)
            {
                throw new InvalidOperationException(
                    "Vehicle does not match the trip.");
            }

            if (vehicle.OrganizationId != driver.OrganizationId)
            {
                throw new InvalidOperationException(
                    "Vehicle and driver belong to different organizations.");
            }

            if (trip.WaktuSelesai.HasValue)
            {
                throw new InvalidOperationException(
                    "Trip already finished.");
            }
        }

         public void ValidateCancelTrip(Trip trip, Driver driver, int driverId)
        {
            if (trip.OrganizationId != driver.OrganizationId)
            {
                throw new InvalidOperationException(
                    "Driver and trip belong to different organizations.");
            }

            if (trip.DriverId != driverId)
            {
                throw new InvalidOperationException(
                    "Driver does not match the trip.");
            }

            if (trip.Status == "Finished")
            {
                throw new InvalidOperationException(
                    "Cannot cancel a finished trip.");
            }

            if (trip.Status == "Canceled")
            {
                throw new InvalidOperationException(
                    "Trip is already canceled.");
            }
        }

        public void ValidateOdometer(int? odometerAkhir, int? odometerAwal)
        {
            if (!odometerAkhir.HasValue)
            {
                throw new InvalidOperationException(
                    "Odometer akhir wajib diisi.");
            }

            if (!odometerAwal.HasValue)
            {
                throw new InvalidOperationException(
                    "Odometer awal tidak tersedia.");
            }

            if (odometerAkhir.Value < odometerAwal.Value)
            {
                throw new InvalidOperationException(
                    "Odometer akhir tidak boleh lebih kecil dari odometer awal.");
            }
        }

    }
}