namespace CarRental.Data.DTOs
{
    public class RegisterCustomerRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string VerificationToken { get; set; } = null!;
    }
}
