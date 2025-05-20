using TrainingApp.Client.Services;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Pages;
public partial class Home
{
    public UserDetailsDTO? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = await userSession.GetUserDetailsAsync();
    }
}
