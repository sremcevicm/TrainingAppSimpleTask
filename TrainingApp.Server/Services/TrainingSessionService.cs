using Microsoft.EntityFrameworkCore;
using TrainingApp.Server.Data.Contexts;
using TrainingApp.Server.Data.Models;
using TrainingApp.Server.Interfaces;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Services
{
    public class TrainingSessionService : ITrainingSession
    {
        private readonly AppDbContext _context;

        public TrainingSessionService(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<ScheduleRequestDTO>> GetAllTrainingSessionsAsync()
        {
            return _context.TrainingSessions
                .Include(ts => ts.User)
                .Select(ts => new ScheduleRequestDTO
                {
                    TrainingSessionId = ts.TrainingSessionId,
                    StartTime = ts.StartTime,
                    DurationInMinutes = (int)(ts.EndTime - ts.StartTime).TotalMinutes,
                    Type = ts.TrainingType,
                    TrainerId = ts.TrainerId,
                    UserId = ts.UserId,
                    FullName = ts.User != null ? ts.User.Name : null,
                    Email = ts.User != null ? ts.User.Email : null
                })
                .ToListAsync();
        }


        public async Task<ScheduleRequestDTO> ScheduleTrainingAsync(ScheduleRequestDTO dto)
        {
            TrainingSession trainingSession;

            if (dto.TrainingSessionId != 0)
            {
                trainingSession = await _context.TrainingSessions
                    .FirstOrDefaultAsync(ts => ts.TrainingSessionId == dto.TrainingSessionId);

                if (trainingSession == null)
                    throw new Exception("Training session not found");
            }
            else
            {
                trainingSession = new TrainingSession();
                _context.TrainingSessions.Add(trainingSession);
            }

            trainingSession.StartTime = dto.StartTime;
            trainingSession.EndTime = dto.StartTime.AddMinutes(dto.DurationInMinutes);
            trainingSession.TrainingType = dto.Type;
            trainingSession.TrainerId = dto.TrainerId;

            User? user = null;
            if (dto.UserId.HasValue)
            {
                user = await _context.Users.FindAsync(dto.UserId.Value);
                if (user == null)
                    throw new Exception("User not found");

                trainingSession.UserId = user.UserId; // Dodeli UserId sesiji
                user.TrainingSessions.Add(trainingSession); // Dodaj trening sesiju korisniku
                _context.Users.Update(user);
            }
            else if (!string.IsNullOrEmpty(dto.FullName) && !string.IsNullOrEmpty(dto.Email))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Name = dto.FullName,
                        Email = dto.Email,
                        PhoneNumber = null,
                        TrainingSessions = new List<TrainingSession>() // Inicijalizuj listu
                    };

                    trainingSession.UserId = user.UserId; // Dodeli UserId sesiji
                    user.TrainingSessions.Add(trainingSession); // Dodaj trening sesiju korisniku
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync(); // Sačuvaj korisnika da bi dobio ID
                }
            }

            await _context.SaveChangesAsync();

            return new ScheduleRequestDTO
            {
                TrainingSessionId = trainingSession.TrainingSessionId,
                StartTime = trainingSession.StartTime,
                DurationInMinutes = (int)(trainingSession.EndTime - trainingSession.StartTime).TotalMinutes,
                Type = trainingSession.TrainingType,
                TrainerId = trainingSession.TrainerId,
                UserId = user?.UserId,
                FullName = user?.Name,
                Email = user?.Email
            };
        }

        public async Task DeleteTrainingSessionAsync(int id)
        {
            var session = await _context.TrainingSessions.FindAsync(id);
            if (session == null)
                throw new Exception("Training session not found.");

            _context.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();
        }


    }
}
