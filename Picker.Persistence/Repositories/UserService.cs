using Picker.Domain.Entities.Users;

namespace Picker.Persistence.Repositories;

public class UserService(IUserRepository userRepository)
{
    private static readonly Random _random = new Random();

    public async Task<string> HandleDickCommandAsync(string username)
    {
        var user = await userRepository.GetUserAsync(username);
        if (user == null)
        {
            var us = new User()
            {
                LastCommandDate = DateTime.MinValue,
                DickSize = 0,
                Username = username,
            };
            await userRepository.CreateUser(us);
        }
        
        if (user.LastCommandDate.HasValue && user.LastCommandDate.Value.Date == DateTime.UtcNow.Date)
        {
            return "завтра дивись";
        }

        
        int change = _random.Next(-10, 11); 
        user.DickSize += change;
        user.LastCommandDate = DateTime.UtcNow;
        await userRepository.UpdateUserAsync(user);
        
        return $"{user.Username} твій пісюн {(change > 0 ? "виріс" : "зменшився")} на {Math.Abs(change)}. Тепер його довжина: {user.DickSize}. Продовжуй грати завтра";
    }

    public async Task<string> GetStats()
    {
        return await userRepository.GetDickTable();
    }
}