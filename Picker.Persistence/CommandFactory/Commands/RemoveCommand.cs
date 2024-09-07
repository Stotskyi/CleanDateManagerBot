using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Telegram.Bot.Types;

namespace Picker.Persistence.CommandFactory.Commands;

public class RemoveCommand : ICommand
{ 
    public Task<string> Execute(UserState userState, Message message)
    {
        userState.State = "awaiting_date_to_remove";
        return Task.FromResult("Видави день");
    }
}