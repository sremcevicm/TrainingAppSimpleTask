using Microsoft.AspNetCore.Components;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Client.Components.Trainer;
public partial class UserEditDialog
{
    [Parameter]
    public UserDetailsDTO Model { get; set; } = new() { Email = string.Empty };

    void OnSubmit()
    {
        DialogService.Close(Model);
    }

}
