using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainingApp.Server.Data.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string AccessCode { get; set; } = string.Empty;

        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    }
}