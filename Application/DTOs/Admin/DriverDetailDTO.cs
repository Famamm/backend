public class DriverDetailDTO
{
    public int DriverId { get; set; }
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? ProfilePhoto { get; set; }
    public string? JenisLisensi { get; set; }
    public string? FotoLisensi { get; set; }
    public DateTimeOffset? TanggalBerlaku { get; set; }
    public string? Status { get; set; }
    public int TotalTrip { get; set; }
}