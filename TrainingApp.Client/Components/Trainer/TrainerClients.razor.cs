using Radzen;
using System.Net.Http.Json;
using TrainingApp.Client.Pages;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Components.Trainer;
public partial class TrainerClients
{
    private List<UserDetailsDTO> clients = new();

    protected override async Task OnInitializedAsync()
    {
        clients = await Http.GetFromJsonAsync<List<UserDetailsDTO>>("api/user");
    }

    async Task OpenEditDialog(UserDetailsDTO editUserData)
    {
        var result = await DialogService.OpenAsync<UserEditDialog>(
            "Izmena korisnika",
            new Dictionary<string, object> { { "Model", editUserData } },
            new DialogOptions { Width = "600px", Height = "auto" });
            
        if (result is UserDetailsDTO userDetails)
        {
             await Http.PutAsJsonAsync<UserDetailsDTO>($"api/user/{editUserData.Email}", userDetails);
        }

    }
}
