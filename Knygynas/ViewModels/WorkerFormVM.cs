using System.ComponentModel.DataAnnotations;

namespace Knygynas.ViewModels;

public class WorkerFormVm
{
    public string? Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
}