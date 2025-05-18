using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Services;

public interface IUserSession
{
    public Task<UserDetailsDTO?> GetUserDetailsAsync();
    public Task<bool> SetUserDetailsAsync(UserDetailsDTO userDetails);
}
