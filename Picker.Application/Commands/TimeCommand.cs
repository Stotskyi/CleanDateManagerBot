using System.Windows.Input;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot.Types;
using ICommand = Picker.Application.Interfaces.ICommand;

namespace Picker.Application.Commands;

public class TimeCommand(IColiverRepository coliverRepository) : ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var response = await coliverRepository.GetRangeOfDate();
        return response;
    }
}
