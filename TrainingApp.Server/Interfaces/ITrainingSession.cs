using TrainingApp.Server.Data.Models;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Interfaces
{
    public interface ITrainingSession
    {
        Task DeleteTrainingSessionAsync(int id);
        Task<List<ScheduleRequestDTO>> GetAllTrainingSessionsAsync();
        Task<ScheduleRequestDTO> ScheduleTrainingAsync(ScheduleRequestDTO dto);
        //Task<ScheduleRequestDTO?> UpdateTrainingAsync(ScheduleRequestDTO dto);
    }
}
