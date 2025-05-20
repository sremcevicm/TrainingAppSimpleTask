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
        private readonly IEmailService _emailService;
        private bool createNew = true;
        private DateTime oldTime;

        public TrainingSessionService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

                createNew = false;
                oldTime = trainingSession.StartTime;
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

                     // Sačuvaj korisnika da bi dobio ID
                }
            }

            trainingSession.UserId = user.UserId;
            user.TrainingSessions.Add(trainingSession);
            trainingSession.User = user; // Dodeli korisnika sesiji
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            var trainerEmail = await _context.Trainers
                .Where(t => t.TrainerId == dto.TrainerId)
                .Select(t => t.Email)
                .FirstOrDefaultAsync();

            if (createNew)
            {
                await _emailService.SendNotificationAsync(user.Email, trainerEmail, $"Trening {dto.Type} je zakazan za {dto.StartTime}. Klijent {dto.FullName}");
            }
            else
            {
                await _emailService.SendNotificationAsync(user.Email, trainerEmail, $"Trening {dto.Type} je pomeren sa {oldTime} na {dto.StartTime}. Klijent {dto.FullName}");

            }

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
            if (session is null)
                throw new Exception("Training session not found.");

            var user = await _context.Users
                .Include(u => u.TrainingSessions)
                .FirstOrDefaultAsync(u => u.UserId == session.UserId);

            if(user is null)
                throw new Exception("User not found.");

            _context.TrainingSessions.Remove(session);
            user.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();

            var trainerEmail = await _context.Trainers
                .Where(t => t.TrainerId == session.TrainerId)
                .Select(t => t.Email)
                .FirstOrDefaultAsync();

            await _emailService.SendNotificationAsync(user.Email, trainerEmail, $"Trening {session.TrainingType} u {session.StartTime} je otkazan.");
        }


    }
}
