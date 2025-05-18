using Microsoft.JSInterop;
using TrainingApp.Shared.DTOs;
using Blazored.LocalStorage;

namespace TrainingApp.Client.Services;

public class UserSession : IUserSession
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILocalStorageService _localStorageService;
    private const string UserDetailsKey = "userDetails";

    public UserSession(IJSRuntime jsRuntime,
                        ILocalStorageService localStorageService)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
    }

    public async Task<UserDetailsDTO?> GetUserDetailsAsync()
    {
        return await _localStorageService.GetItemAsync<UserDetailsDTO>(UserDetailsKey);
    }

    public async Task<bool> SetUserDetailsAsync(UserDetailsDTO userDetails)
    {
        try
        {
            await _localStorageService.SetItemAsync(UserDetailsKey, userDetails);
            return true;
        }
        catch
        {
            return false;
        }
    }
}