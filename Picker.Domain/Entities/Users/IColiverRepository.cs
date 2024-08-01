namespace Picker.Domain.Entities.Users;

public interface IColiverRepository
{
    public Task<string> WriteColiverAsync(string date, string username);
    public Task<byte[]> GetCleanersTable();
    
    public Task<string> GetCleanerToday(DateOnly date);

    public Task<(DateOnly startDate, DateOnly currentTime)> CreateCycle(byte count);
    
    Task<string> RemoveFromTable(string day, string? username);
}