using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Components.Trainer;
public partial class TrainerLoginDialog
{
    [Parameter]
    public UserDetailsDTO Model { get; set; } = new() { Email = string.Empty };
    private string trainerCode = string.Empty;

    async Task OnSubmit()
    {
        if (string.IsNullOrEmpty(trainerCode))
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Unesite kod trenera");
            return;
        }

        try
        {
            var encodedCode = Uri.EscapeDataString(trainerCode);
            var response = await Http.GetAsync($"api/trainer/{encodedCode}");

            if (response.IsSuccessStatusCode)
            {
                Model = await response.Content.ReadFromJsonAsync<UserDetailsDTO>();
                DialogService.Close(Model);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Greška", "Kod trenera nije pronađen");
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", $"Dogodila se greška: {ex.Message}");
        }

    }
}
