using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Interfaces
{
    public interface IUser
    {
        Task<UserDetailsDTO?> GetUserByEmailAsync(string email);
        Task<UserDetailsDTO> AddUserAsync(UserDetailsDTO user);
        Task<bool> UpdateUserAsync(UserDetailsDTO user);
        Task<List<UserDetailsDTO>> GetAllUsersAsync();
    }
}
