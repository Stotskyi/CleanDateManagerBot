namespace Picker.Infrastructure.Entities;

public class Coliver
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    
    
    public CleaningTime? CleaningTime { get; set; }
}