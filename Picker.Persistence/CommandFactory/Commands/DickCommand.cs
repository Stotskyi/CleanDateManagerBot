using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Picker.Persistence.Repositories;
using Telegram.Bot.Types;

namespace Picker.Persistence.CommandFactory.Commands;

public class DickCommand(UserService userService) : IDickCommand
{
    public async Task<string> Execute(string username,string firstname,string lastname)
    {
        return await userService.HandleDickCommandAsync(username,firstname,lastname);
    }
}