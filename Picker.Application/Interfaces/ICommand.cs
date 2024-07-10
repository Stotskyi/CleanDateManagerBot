using Picker.Infrastructure.Entities;
using Telegram.Bot.Types;

namespace Picker.Application.Interfaces;

public interface ICommand
{
    Task<string> Execute(UserState userState, Message message);
}