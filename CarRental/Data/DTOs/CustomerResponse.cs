using CarRental.Data.Models;

namespace CarRental.Data.DTOs
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public string Role { get; set; } = null!;
        public List<RentalResponse> Rentals { get; set; } = new();
    }
}
