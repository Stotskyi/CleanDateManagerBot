using System.ComponentModel.DataAnnotations;

namespace Picker.Infrastructure.Entities;

public class UserState
{
    [Key]
    public long UserId { get; set; }
    public string? State { get; set; }
    public string? LastCommand { get; set; }
    public DateTime? LastInteraction { get; set; }
    public string? InputDate { get; set; }
}