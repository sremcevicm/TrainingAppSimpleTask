using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Server.Data.Contexts;
using TrainingApp.Server.Data.Models;
using TrainingApp.Server.Interfaces;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Services
{
    public class TrainerService : ITrainer
    {
        private readonly AppDbContext _context;
        public TrainerService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ChooseTrainerDTO>> GetAllTrainersAsync()
        {
            return await _context.Trainers
            .Select(t => new ChooseTrainerDTO
            {
                Id = t.TrainerId,
                FullName = t.Name,
                Email = t.Email,
                CancellationNoticeInHours = t.CancellationNoticeInHours
            })
            .ToListAsync();
        }

        public async Task<UserDetailsDTO?> GetTrainerByCodeAsync(string hashedCode)
        {
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.AccessCode == hashedCode);

            return trainer == null ? null : new UserDetailsDTO
            {
                Name = trainer.Name,
                Email = trainer.Email,
                IsTrainer = true
            };
        }

        public async Task<bool> UpdateCancellationNoticeAsync(int trainerId, int hours)
        {
            var trainer = await _context.Trainers.FindAsync(trainerId);
            if (trainer == null)
                return false;

            trainer.CancellationNoticeInHours = hours;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
