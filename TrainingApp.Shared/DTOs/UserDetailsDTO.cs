namespace TrainingApp.Shared.DTOs;

public class UserDetailsDTO
{
    public string Name { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsTrainer { get; set; } = false;
}
