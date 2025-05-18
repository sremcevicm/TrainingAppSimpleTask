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
                FullName = t.Name
            })
            .ToListAsync();
        }

        public async Task<UserDetailsDTO> GetTrainerByCodeAsync(string code)
        {
            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(t => t.AccessCode == code);

            if (trainer != null)
            {
                var userDetails = new UserDetailsDTO
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    IsTrainer = true
                };
                return userDetails;
            }
            else
            {
                return new UserDetailsDTO
                {
                    Name = string.Empty,
                    Email = string.Empty,
                    IsTrainer = false
                };
            }
        }
    }
}
