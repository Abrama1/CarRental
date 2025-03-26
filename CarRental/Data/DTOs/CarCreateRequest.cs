namespace CarRental.Data.DTOs
{
    public class CarCreateRequest
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal DailyRate { get; set; }
        public string Location { get; set; } = null!;
        public string? LicensePlate { get; set; }
        public string? ImageUrl { get; set; }
    }

}
