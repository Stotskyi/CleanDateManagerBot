using Picker.Domain.Entities.Users;
using Telegram.Bot.Types;
using User = Picker.Domain.Entities.Users.User;

namespace Picker.Persistence.Repositories;

public class UserService(IUserRepository userRepository)
{
    private static readonly Random _random = new Random();

    public async Task<string> HandleDickCommandAsync(string username,string firstname,string lastname)
    {
        var user = await userRepository.GetUserAsync(username);
        if (user == null)
        {
            var us = new User()
            {
                FirstName = firstname,
                LastName = lastname,
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

        
        var randomNumber = GetRandomValue();
        user.DickSize += randomNumber;
        user.LastCommandDate = DateTime.UtcNow;
        await userRepository.UpdateUserAsync(user);
        
        return $"{user.Username} твій пісюн {(user.DickSize > 0 ? "виріс" : "зменшився")} на {Math.Abs(randomNumber)}. Тепер його довжина: {user.DickSize}. Продовжуй грати завтра";
    }

    public async Task<string> GetStats()
    {
        return await userRepository.GetDickTable();
    }
    
    int GetRandomValue()
    {
        double randomValue = _random.NextDouble(); 
        int result = randomValue switch
        {
            <= 0.0000001 => 1000, 
            <= 0.01 => 100,      
            <= 0.11 => 20,        
            <= 0.21 => -50,        
            <= 0.22 => -15,        
            <= 0.31 => 50,         
            _ => _random.Next(-20, 24)  
        };
        
        if (result == 0)
        {
            result = _random.Next(1, 3) == 1 ? -1 : 1; 
        }

        return result;
    }
}
