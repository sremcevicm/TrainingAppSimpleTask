using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using System.ComponentModel;
using System.Net.Http.Json;
using TrainingApp.Shared.DTOs;
using static System.Net.WebRequestMethods;

namespace TrainingApp.Client.Components.Appointments;
public partial class ScheduleDialog
{
    [Parameter]
    public ScheduleRequestDTO Model { get; set; }

    [Parameter]
    public bool isByTrainer { get; set; }
    bool isDisabled = false;
    bool isCancelPossible = true;

    [Parameter]
    public ChooseTrainerDTO? Trainer { get; set; }

    private List<dynamic> durations = new()
    {
        new { Text = "30 minuta", Value = 30 },
        new { Text = "60 minuta", Value = 60 }
    };

    protected override async Task OnInitializedAsync()
    {
        // trainers = await Http.GetFromJsonAsync<List<ChooseTrainerDTO>>("api/trainers");
        if (Model.TrainingSessionId != 0)
        {
            isDisabled = true;
        }

        // Fix for CS0019: Convert CancellationNoticeInHours to TimeSpan before subtracting
        // Fix for CS8602: Add null check for Trainer
        if (Trainer != null && Model.StartTime - TimeSpan.FromHours(Trainer.CancellationNoticeInHours) < DateTime.Now)
        {
            isCancelPossible = false; // Assuming this is the intended logic
        }
    }

    void OnSubmit()
    {
        DialogService.Close(Model);
    }

    void OnCancel()
    {
        Model.IsCanceled = true;
        DialogService.Close(Model);
    }

}
