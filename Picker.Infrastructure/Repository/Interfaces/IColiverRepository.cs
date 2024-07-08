namespace Picker.Infrastructure.Repository.Interfaces;

public interface IColiverRepository
{
    public Task<string> WriteColiverAsync(string date, string username);
    public Task<byte[]> GetCleanersTable();
    public Task<string> GetTextTable();
    public Task<string> GetCleanerToday(DateOnly date);

    public Task<(DateOnly startDate, DateOnly currentTime)> CreateCycle(byte count);

    public Task<string> GetRangeOfDate();
    Task<string> RemoveFromTable(string day, string? username);
}