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
            var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #3498db; border-radius: 10px; max-width: 500px; margin: auto; background-color: #f4faff;'>
                <h2 style='color: #2c3e50; margin-top: 0;'>🔐 Email Verification</h2>
                <p>Thank you for signing up with <strong>CarRental Inc.</strong></p>
                <p>To complete your registration, please use the verification code below:</p>

                <div style='text-align: center; margin: 30px 0;'>
                    <span style='display: inline-block; background-color: #3498db; color: #fff; padding: 15px 30px; font-size: 24px; letter-spacing: 3px; border-radius: 8px; font-weight: bold;'>
                        {token}
                    </span>
                </div>

                <p>This code will expire in 10 minutes. If you did not request this, please ignore this email.</p>

                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ccc;' />
                <footer style='font-size: 0.9em; color: #777; text-align: center;'>
                    <p>Need help? <a href='mailto:support@carrental.com' style='color: #3498db;'>Contact support</a></p>
                    <p>CarRental Inc. • All rights reserved</p>
                </footer>
            </div>";


            var mailMessage = new MailMessage(_fromAddress, toEmail, subject, body)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromAddress, toEmail, subject, body)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }

    }
}