using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;
using TrainingApp.Client.Components.Appointments;
using TrainingApp.Client.Components.Trainer;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Pages;

public partial class UserDetails
{
    public UserDetailsDTO Model { get; set; } = new() { Email = string.Empty };
    private bool isDisabled = false;

    protected override async Task OnInitializedAsync()
    {
        Model = await userSession.GetUserDetailsAsync() ?? new() { Email = string.Empty };
        if(!string.IsNullOrEmpty(Model.Email))
        {
            isDisabled = true;
        }
    }

    private async Task OnSubmit()
    {
        try
        {
            // Pozovi backend da proveri/doda korisnika
            var response = await Http.PostAsJsonAsync("api/user/check-or-add", Model);

            if (response.IsSuccessStatusCode)
            {
                // Uspesno je korisnik verifikovan ili dodat
                var updatedUser = await response.Content.ReadFromJsonAsync<UserDetailsDTO>();

                if (updatedUser != null)
                {
                    Model = updatedUser;

                    // Snimi podatke lokalno
                    var result = await userSession.SetUserDetailsAsync(Model);
                    if (result)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Uspešno ste se prijavili", "Uspešno ste se prijavili");
                        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                    }
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Greška", "Neuspešan odgovor sa servera");
                }
            }
            else
            {
                // Server vratio grešku (npr. 400, 500...)
                var error = await response.Content.ReadAsStringAsync();
                NotificationService.Notify(NotificationSeverity.Error, "Greška", $"Server je vratio grešku: {error}");
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", $"Dogodila se greška: {ex.Message}");
        }
    }


    private async Task OnUserChange()
    {
        // Prazni model
        Model = new() { Email = string.Empty };

        // Izmeni localStorage ili gde god cuvas podatke
        var result = await userSession.SetUserDetailsAsync(Model);

        if (result)
        {
            // Osveži stranicu
            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        }
        else
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Nije moguće izbrisati podatke");
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

        if (result != null)
        {
            Model = result; // ovde ažuriraš svoj lokalni Model sa vrednošću iz dijaloga
            var setResult = await userSession.SetUserDetailsAsync(Model);
            if (setResult)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Ulogovali ste se kao trener");
                StateHasChanged();
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Greška", "Greška prilikom ažuriranja podataka");
            }
        }
        else
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Trener nije pronađen ili prijava otkazana");
        }

    }
}