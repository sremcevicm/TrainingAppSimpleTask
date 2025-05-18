using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrainingApp.Shared.DTOs
{
    public class ScheduleRequestDTO
    {
        public int TrainingSessionId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationInMinutes { get; set; }
        [JsonIgnore]
        public DateTime EndTime => StartTime.AddMinutes(DurationInMinutes);

        public string Type { get; set; }
        public bool IsCanceled { get; set; }

        // User info (može biti null ako termin nije zauzet)
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int TrainerId { get; set; }
        public string? TrainerFullName { get; set; } // opcionalno za prikaz

    }

}
