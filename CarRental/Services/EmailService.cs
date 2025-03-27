using CarRental.Interfaces;
using System.Net;
using System.Net.Mail;

namespace CarRental.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;

        public EmailService(IConfiguration config)
        {
            _fromAddress = config["EmailSettings:From"]!;
            _smtpClient = new SmtpClient(config["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(config["EmailSettings:SmtpPort"]!),
                Credentials = new NetworkCredential(
                    config["EmailSettings:SmtpUser"],
                    config["EmailSettings:SmtpPass"]
                ),
                EnableSsl = true
            };
        }

        public async Task SendVerificationEmailAsync(string toEmail, string token)
        {
            var subject = "Verify your email";
            var body = $"Your verification code is: {token}";

            var mailMessage = new MailMessage(_fromAddress, toEmail, subject, body);
            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromAddress, toEmail, subject, body);
            await _smtpClient.SendMailAsync(mailMessage);
        }

    }
}