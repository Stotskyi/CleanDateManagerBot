using Picker.Application.Data;
using Picker.Persistence.Repositories;

namespace Picker.Persistence.CommandFactory.Commands;

public class DickRatingCommand(UserService userService) : IScheduleCommand
{
    public async Task<string> Execute()
    {
        return await userService.GetStats();
    }
}
