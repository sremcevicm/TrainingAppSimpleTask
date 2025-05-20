using System.Net.Mail;
using System.Net;
using TrainingApp.Server.Interfaces;

namespace TrainingApp.Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendNotificationAsync(string clientEmail, string trainerEmail, string message)
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:Sender"]),
                Subject = "Obaveštenje o trening terminu",
                Body = message,
                IsBodyHtml = false
            };

            if (!string.IsNullOrWhiteSpace(clientEmail))
                mailMessage.To.Add(clientEmail);

            if (!string.IsNullOrWhiteSpace(trainerEmail))
                mailMessage.To.Add(trainerEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
