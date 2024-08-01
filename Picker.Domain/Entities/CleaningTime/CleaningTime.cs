
namespace Picker.Domain.Entities.CleaningTime;

public class CleaningTime
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    
    public List<Coliver>? Colivers { get; set; }
    
    public int Cycle { get; set; }
}
