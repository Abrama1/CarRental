namespace CarRental.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string token);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}