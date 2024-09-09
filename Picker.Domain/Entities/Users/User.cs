using System.ComponentModel.DataAnnotations;

namespace Picker.Domain.Entities.Users;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public int DickSize { get; set; } 
    public DateTime? LastCommandDate { get; set; }
    public string? LastName { get; set; } = String.Empty;
    public string FirstName { get; set; }
}