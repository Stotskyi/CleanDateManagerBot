using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Picker.Persistence.Repositories;

namespace Picker.Persistence.CommandFactory.Commands;

public class DickCommand(UserService userService) : IDickCommand
{
    public async Task<string> Execute(string username)
    {
        return await userService.HandleDickCommandAsync(username);
    }
}