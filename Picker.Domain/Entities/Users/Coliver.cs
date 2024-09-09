
using Picker.Domain.Entities.CleaningTime;

public class Coliver
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    
    public CleaningTime? CleaningTime { get; set; }
    public string? LastName { get; set; } = String.Empty;
    public string FirstName { get; set; }
}