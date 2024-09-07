namespace Picker.Domain.Entities.Users;

public interface IUserRepository
{
    Task<User?> GetUserAsync(string username);
    Task UpdateUserAsync(User user);
    Task<string> GetDickTable();
    public Task CreateUser(User user);
}