using Picker.Application.Interfaces;
using Picker.Infrastructure.Entities;
using Telegram.Bot.Types;

namespace Picker.Application.Commands;

public class RemoveCommand : ICommand
{
    public Task<string> Execute(UserState userState, Message message)
    {
        userState.State = "awaiting_date_to_remove";
        return Task.FromResult("Видави день");
    }
}