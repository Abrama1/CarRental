using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Data.Models
{
    public class Rental
    {
        public int Id { get; set; }

        // Foreign Keys
        public int CarId { get; set; }
        public int CustomerId { get; set; }

        // Navigation
        public Car Car { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

        // Booking Info
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }
        public RentalStatus Status { get; set; } = RentalStatus.Reserved;
    }
}
