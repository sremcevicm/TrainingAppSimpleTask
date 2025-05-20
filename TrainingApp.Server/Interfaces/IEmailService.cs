namespace TrainingApp.Server.Interfaces
{
    public interface IEmailService
    {
        Task SendNotificationAsync(string clientEmail, string trainerEmail, string message);
    }
}
