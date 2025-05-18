using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using System.Net.Http.Json;
using TrainingApp.Shared.DTOs;
using static System.Net.WebRequestMethods;

namespace TrainingApp.Client.Components.Appointments;
public partial class ScheduleDialog
{
    [Parameter]
    public ScheduleRequestDTO Model { get; set; }
    private List<ChooseTrainerDTO> trainers = new();
    bool isDisabled = false;

    private List<dynamic> durations = new()
    {
        new { Text = "30 minuta", Value = 30 },
        new { Text = "60 minuta", Value = 60 }
    };

    protected override async Task OnInitializedAsync()
    {
        //trainers = await Http.GetFromJsonAsync<List<ChooseTrainerDTO>>("api/trainers");
        trainers = await Http.GetFromJsonAsync<List<ChooseTrainerDTO>>("api/trainer");
        if(Model.TrainingSessionId != 0)
        {
            isDisabled = true;
        }
    }

    void OnSubmit()
    {
        DialogService.Close(Model);
    }
}
