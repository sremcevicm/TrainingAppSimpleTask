using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Components.Trainer;
public partial class TrainerLoginDialog
{
    [Parameter]
    public UserDetailsDTO Model { get; set; } = new();
    private string trainerCode = string.Empty;

    async Task OnSubmit()
    {
        if (string.IsNullOrEmpty(trainerCode))
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Unesite kod trenera");
            return;
        }

        Model = await Http.GetFromJsonAsync<UserDetailsDTO>($"api/trainer/{trainerCode}");
        if (Model is null)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Kod trenera nije pronađen");
            return;
        }
        DialogService.Close(Model);
    }
}
