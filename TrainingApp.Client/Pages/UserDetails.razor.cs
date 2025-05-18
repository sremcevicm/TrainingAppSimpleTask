using Radzen;
using TrainingApp.Client.Components.Appointments;
using TrainingApp.Client.Components.Trainer;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Pages;

public partial class UserDetails
{
    public UserDetailsDTO Model { get; set; } = new();
    private bool isDisabled = false;

    protected override async Task OnInitializedAsync()
    {
        Model = await userSession.GetUserDetailsAsync() ?? new();
        if(!string.IsNullOrEmpty(Model.Email))
        {
            isDisabled = true;
        }
        //Model = await Http.GetFromJsonAsync<UserDetailsDTO>($"api/user/{userEmail}");
    }

    private async Task OnSubmit()
    {
        var result = await userSession.SetUserDetailsAsync(Model);
        if (result)
        {
            
        }
        else
        {
            // Handle error
        }
    }

    private async Task OpenTrainerDialog()
    {
        var result = await DialogService.OpenAsync<TrainerLoginDialog>(
            "Prijava trenera",
            new Dictionary<string, object> 
            {
            { "Model", Model }
            },
            new DialogOptions { Width = "auto", Height = "auto" });

        result = await userSession.SetUserDetailsAsync(Model);
        if (result)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Ulogovali ste se kao trener");
            StateHasChanged();
        }
        else
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Trener nije pronađen");
        }

    }
}