using System.ComponentModel.DataAnnotations;

namespace CarRental.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; } = null!;
        [Required][EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Phone { get; set; } = null!;
        public DateTime JoinDate { get; set; }

        // Authentication
        [Required] public string PasswordHash { get; set; } = null!;

        // Email verification
        public bool IsVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
        public DateTime? TokenExpiry { get; set; }

        // Role handling
        public string Role { get; set; } = "Customer";

        public bool IsDeleted { get; set; } = false;

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
