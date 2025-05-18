using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrainingApp.Shared.DTOs;
using static System.Net.WebRequestMethods;

namespace TrainingApp.Client.Components.Appointments;
public partial class Schedule
{
    RadzenScheduler<ScheduleRequestDTO> scheduler;
    private List<ScheduleRequestDTO> scheduleRequests = new();
    private int? TrainerId { get; set; } = 0;
    private List<ChooseTrainerDTO> trainers = new();

    private UserDetailsDTO userDetails = new();
    //private string userEmail = string.Empty;

    [Inject]
    private IJSRuntime JS { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //userEmail = await JS.InvokeAsync<string>("localStorageHelper.getItem", "userEmail");
        userDetails = await userSession.GetUserDetailsAsync() ?? new();
        trainers = await Http.GetFromJsonAsync<List<ChooseTrainerDTO>>("api/trainer");
        await LoadAndFilterSessions();
    }

    private async Task OnTrainerChanged(object value)
    {
        TrainerId = value as int?;
        await LoadAndFilterSessions();
    }

    private async Task LoadAndFilterSessions()
    {
        var allSessions = await Http.GetFromJsonAsync<List<ScheduleRequestDTO>>("api/trainingsession");

        //userEmail = await JS.InvokeAsync<string>("localStorage.getItem", "userEmail");

        if (TrainerId != 0)
        {
            scheduleRequests = allSessions
                .Where(x => x.TrainerId == TrainerId)
                .ToList();
        }
        else
        {
            scheduleRequests = new();
        }
    }
    void OnSlotRender(SchedulerSlotRenderEventArgs args)
    {
        // Highlight working hours (8-23)
        if ((args.View.Text == "Week" || args.View.Text == "Day") && args.Start.Hour > 8 && args.Start.Hour < 23)
        {
            args.Attributes["style"] = "background: var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2));";
        }
    }

    void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<ScheduleRequestDTO> args)
    {
        // Never call StateHasChanged in AppointmentRender - would lead to infinite loop

        if (args.Data.IsCanceled)
        {
            args.Attributes["style"] = "background: red";
        }
        else if (args.Data.StartTime < DateTime.Now)
        {
            args.Attributes["style"] = "background: gray";
        }
        else
        {
            args.Attributes["style"] = "background: blue";
        }
    }

    async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<ScheduleRequestDTO> args)
    {
        var model = args.Data;
        model.TrainerId = TrainerId ?? 0;

        var result = await DialogService.OpenAsync<ScheduleDialog>(
            "Pregled termina",
            new Dictionary<string, object>
            {
            { "Model", model }
            },
            new DialogOptions { Width = "auto", Height = "auto" });

        //if (result is not null)
        //{
        //    var response = await Http.PutAsJsonAsync($"api/trainingsession/schedule/{model.TrainingSessionId}", model);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Ažuriramo postojeći element u listi
        //        var index = scheduleRequests.FindIndex(r => r.TrainingSessionId == model.TrainingSessionId);
        //        if (index != -1)
        //        {
        //            scheduleRequests[index] = model;
        //        }

        //        NotificationService.Notify(NotificationSeverity.Success, "Uspešno izmenjen trening", model.Type);
        //        await scheduler.Reload();
        //    }
        //    else
        //    {
        //        NotificationService.Notify(NotificationSeverity.Error, "Greška", "Nije uspela izmena treninga.");
        //    }
        //}
    }


    async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
    {
        var model = new ScheduleRequestDTO
        {
            FullName = userDetails.Name,
            Email = userDetails.Email,
            StartTime = args.Start,
            TrainerId = TrainerId ?? 0
        };

        var result = await DialogService.OpenAsync<ScheduleDialog>(
            "Zakazivanje termina",
            new Dictionary<string, object>
            {
        { "Model", model }
            },
            new DialogOptions { Width = "auto", Height = "auto" });

        if (result is not null)
        {
            var response = await Http.PostAsJsonAsync("api/trainingsession/schedule", model);

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadFromJsonAsync<ScheduleRequestDTO>();
                scheduleRequests.Add(result!);
                NotificationService.Notify(NotificationSeverity.Success, "Zakazali ste trening: ", result!.Type);
                //userEmail = result.Email;
                //await SaveUserToLocalStorageAsync(userEmail);
                StateHasChanged();
                await scheduler.Reload();
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to schedule training.");
            }
        }

    }

    async Task OnAppointmentMove(SchedulerAppointmentMoveEventArgs args)
    {
        var draggedAppointment = scheduleRequests.FirstOrDefault(x => x == args.Appointment.Data);

        if (draggedAppointment is not null)
        {
            DateTime newStartTime;
            if (args.SlotDate.TimeOfDay == TimeSpan.Zero)
            {
                var originalTime = draggedAppointment.StartTime.TimeOfDay;
                var roundedMinutes = (int)(Math.Round(originalTime.TotalMinutes / 30.0) * 30) % 1440;
                var roundedTime = TimeSpan.FromMinutes(roundedMinutes);

                newStartTime = args.SlotDate.Date.Add(roundedTime);
            }
            else
            {
                newStartTime = args.SlotDate;
            }

            var newEndTime = newStartTime.AddMinutes(draggedAppointment.DurationInMinutes);

            // Check for overlapping intervals (excluding the current appointment)
            bool isOverlapping = scheduleRequests
                .Where(x => x.TrainingSessionId != draggedAppointment.TrainingSessionId)
                .Any(x =>
                {
                    var existingStart = x.StartTime;
                    var existingEnd = x.StartTime.AddMinutes(x.DurationInMinutes);
                    return newStartTime < existingEnd && newEndTime > existingStart;
                });

            if (isOverlapping)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Greška", "Termin se poklapa sa postojećim terminom.");
                return;
            }

            draggedAppointment.StartTime = newStartTime;

            var response = await Http.PutAsJsonAsync($"api/trainingsession/schedule/{draggedAppointment.TrainingSessionId}", draggedAppointment);
            if (response.IsSuccessStatusCode)
            {
                var index = scheduleRequests.FindIndex(r => r.TrainingSessionId == draggedAppointment.TrainingSessionId);
                if (index != -1)
                {
                    scheduleRequests[index] = draggedAppointment;
                }

                NotificationService.Notify(NotificationSeverity.Success, "Uspešno izmenjeno vreme", draggedAppointment.Type);
                await scheduler.Reload();
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Greška", "Nije uspela izmena termina.");
            }

            await scheduler.Reload();
        }
    }

    private async Task SaveUserToLocalStorageAsync(string email)
    {
        var existing = await JS.InvokeAsync<string>("localStorage.getItem", "userEmail");
        if (string.IsNullOrEmpty(existing))
        {
            await JS.InvokeVoidAsync("localStorage.setItem", "userEmail", email);
            Console.WriteLine("Korisnik sačuvan u localStorage.");
        }
        else
        {
            Console.WriteLine("Korisnik već postoji u localStorage.");
        }
    }

    private async Task<string> GetUserFromLocalStorageAsync()
    {
        return await JS.InvokeAsync<string>("localStorage.getItem", "userEmail");
    }

}
