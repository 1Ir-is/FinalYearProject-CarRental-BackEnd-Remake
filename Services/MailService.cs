using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CarRental_BE.Interfaces;
using CarRental_BE.Models.Auth;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CarRental_BE.Services
{
    public class MailService : IMailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<MailService> logger;

        public MailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail))
                {
                    throw new ArgumentException("Recipient email address is required", nameof(toEmail));
                }

                var mail = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Mail, _emailSettings.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(toEmail)); // Add recipient's email address here

                using (var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                // Log any errors
                Console.WriteLine(ex.Message);
                throw;
            }
        }


    }
}
