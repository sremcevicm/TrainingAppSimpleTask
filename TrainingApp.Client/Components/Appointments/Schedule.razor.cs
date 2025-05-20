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
    private int CancellationNoticeInHours { get; set; } = 0;
    private List<ChooseTrainerDTO> trainers = new();

    [Parameter]
    public UserDetailsDTO? UserDetails { get; set; }
    //private string userEmail = string.Empty;

    [Inject]
    private IJSRuntime JS { get; set; }

    protected override async Task OnInitializedAsync()
    {
        trainers = await Http.GetFromJsonAsync<List<ChooseTrainerDTO>>("api/trainer");

        // Ako korisnik nije trener, možeš automatski postaviti njegovog trenera ako znaš Email
        if (UserDetails.IsTrainer && !string.IsNullOrEmpty(UserDetails.Email))
        {
            var trainer = trainers?.FirstOrDefault(t => t.Email == UserDetails.Email);
            if (trainer != null)
            {
                TrainerId = trainer.Id;
            }
            CancellationNoticeInHours = trainer.CancellationNoticeInHours;
        }

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

        if (allSessions == null)
        {
            scheduleRequests = new List<ScheduleRequestDTO>();
            return;
        }

        scheduleRequests = allSessions
                .Where(x => x.TrainerId == TrainerId)
                .ToList();
    }
    private async Task OnHoursChange()
    {
        if (TrainerId is null)
            return;

        var response = await Http.PutAsJsonAsync(
            $"api/trainer/{TrainerId}/cancellation-notice",
            CancellationNoticeInHours
        );

        if (response.IsSuccessStatusCode)
        {
            var trainer = trainers.FirstOrDefault(t => t.Id == TrainerId);
            if (trainer != null)
            {
                trainer.CancellationNoticeInHours = CancellationNoticeInHours;
            }
            NotificationService.Notify(NotificationSeverity.Success, "Uspeh", "Otkazni rok je ažuriran.");
            await scheduler.Reload();
        }
        else
        {
            NotificationService.Notify(NotificationSeverity.Error, "Greška", "Nije uspelo ažuriranje otkaznog roka.");
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

        if (args.Data.StartTime < DateTime.Now)
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

        // Find the trainer with the matching ID
        var selectedTrainer = trainers.FirstOrDefault(t => t.Id == model.TrainerId);

        var result = await DialogService.OpenAsync<ScheduleDialog>(
            "Pregled termina",
            new Dictionary<string, object>
            {
                { "Model", model },
                { "Trainer", selectedTrainer }
            },
            new DialogOptions { Width = "auto", Height = "auto" });

        if (result is ScheduleRequestDTO updatedModel)
        {
            if (updatedModel.IsCanceled)
            {
                // Ovde brišemo termin
                var response = await Http.DeleteAsync($"api/trainingsession/{updatedModel.TrainingSessionId}");

                if (response.IsSuccessStatusCode)
                {
                    // Ukloni iz lokalne liste
                    scheduleRequests.RemoveAll(r => r.TrainingSessionId == updatedModel.TrainingSessionId);

                    NotificationService.Notify(NotificationSeverity.Warning, "Termin otkazan", updatedModel.Type);
                    await scheduler.Reload();
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Greška", "Nije uspelo otkazivanje termina.");
                }
            }
            else
            {
                // Ako nije otkazivanje, možeš ovde vratiti stari PUT ako ti treba
            }
        }
    }


    async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
    {
        if (args.Start.Date <= DateTime.Today)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Zakazivanje nije dozvoljeno", "Možete zakazati termin samo od sutra.");
            return;
        }

        var model = new ScheduleRequestDTO
        {
            FullName = UserDetails.IsTrainer ? string.Empty : UserDetails.Name,
            Email = UserDetails.IsTrainer ? string.Empty : UserDetails.Email,
            StartTime = args.Start,
            TrainerId = TrainerId ?? 0
        };

        var result = await DialogService.OpenAsync<ScheduleDialog>(
            "Zakazivanje termina",
            new Dictionary<string, object>
            {
        { "Model", model },
        { "isByTrainer", UserDetails.IsTrainer }
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


}
