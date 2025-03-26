using System.ComponentModel.DataAnnotations;

namespace CarRental.Data.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required] public string Username { get; set; } = null!;
        [Required] public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Admin";
    }
}
