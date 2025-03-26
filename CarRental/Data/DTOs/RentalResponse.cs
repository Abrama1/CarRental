using CarRental.Data.Models;

namespace CarRental.Data.DTOs
{
    public class RentalResponse
    {
        public int Id { get; set; }
        public string CarName { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RentalStatus Status { get; set; }
    }
}
