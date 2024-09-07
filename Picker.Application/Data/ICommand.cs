using Picker.Domain.Entities.Users;
using Telegram.Bot.Types;

namespace Picker.Application.Data;

public interface ICommand
{
    Task<string> Execute(UserState userState, Message message);
}
