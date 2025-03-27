using CarRental.Data.Models;

namespace CarRental.Data.DTOs
{
    public class RentalResponse
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarMake { get; set; } = null!;
        public string CarModel { get; set; } = null!;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
    }
}
