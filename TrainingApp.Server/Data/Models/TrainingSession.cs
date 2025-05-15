using System.ComponentModel.DataAnnotations;

namespace TrainingApp.Server.Data.Models
{
    public class TrainingSession
    {
        public int TrainingSessionId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [StringLength(100)]
        public string TrainingType { get; set; } = string.Empty; // Yoga, Gym, etc.

        [Required]
        public int TrainerId { get; set; }

        public Trainer? Trainer { get; set; }

        public ICollection<UserTraining> UserTrainings { get; set; } = new List<UserTraining>();
    }
}
