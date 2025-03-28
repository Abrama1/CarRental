using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required] public string Make { get; set; } = null!;
        [Required] public string Model { get; set; } = null!;
        public int Year { get; set; }
        public decimal DailyRate { get; set; }
        public string Location { get; set; } = null!;
        public bool IsAvailable { get; set; } = true;
        public string? LicensePlate { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
