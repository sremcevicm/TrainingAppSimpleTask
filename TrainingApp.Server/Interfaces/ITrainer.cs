using TrainingApp.Server.Data.Models;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Interfaces
{
    public interface ITrainer
    {
        Task<List<ChooseTrainerDTO>> GetAllTrainersAsync();
        Task<UserDetailsDTO> GetTrainerByCodeAsync(string code);
    }
}
