using System.ComponentModel.DataAnnotations;

namespace TrainingApp.Server.Data.Models
{
    public class UserTraining
    {
        public int UserTrainingId { get; set; }

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        [Required]
        public int TrainingSessionId { get; set; }

        public TrainingSession? TrainingSession { get; set; }
    }
}