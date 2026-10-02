namespace fleetops_backend.Application.DTOs
{
    // Fleet manager tidak butuh field tambahan di luar data user dasar —
    // tidak punya SIM maupun kendaraan yang di-assign.
    public class CreateFleetManagerDTO : CreateUserDTO
    {
        public string? PhoneNumber { get; set; }
    }
}
