using TrainingApp.Server.Data.Models;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Interfaces
{
    public interface ITrainingSession
    {
        Task<List<ScheduleRequestDTO>> GetAllTrainingSessionsAsync();
        Task<ScheduleRequestDTO> ScheduleTrainingAsync(ScheduleRequestDTO dto);
        //Task<ScheduleRequestDTO?> UpdateTrainingAsync(ScheduleRequestDTO dto);
    }
}
