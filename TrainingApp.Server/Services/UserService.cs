using Microsoft.EntityFrameworkCore;
using TrainingApp.Server.Data.Contexts;
using TrainingApp.Server.Data.Models;
using TrainingApp.Server.Interfaces;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Services
{
    public class UserService : IUser
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserDetailsDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            return new UserDetailsDTO
            {
                Name = user.Name,
                Email = user.Email,
                IsTrainer = false
            };
        }

        public async Task<UserDetailsDTO> AddUserAsync(UserDetailsDTO userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PhoneNumber = userDto.Phone,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDetailsDTO
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                IsTrainer = false
            };
        }

        public async Task<List<UserDetailsDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => new UserDetailsDTO
            {
                Name = u.Name,
                Email = u.Email,
                Phone = u.PhoneNumber ?? string.Empty,
                IsTrainer = false
            }).ToList();
        }


        public Task<bool> UpdateUserAsync(UserDetailsDTO user)
        {
            throw new NotImplementedException();
        }
    }
}
