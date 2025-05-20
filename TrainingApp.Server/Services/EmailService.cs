using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using TrainingApp.Server.Interfaces;

namespace TrainingApp.Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }

        public async Task SendNotificationAsync(string clientEmail, string trainerEmail, string message)
        {
            using var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Sender),
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
