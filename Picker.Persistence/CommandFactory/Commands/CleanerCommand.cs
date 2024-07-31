using System.Windows.Input;
using Picker.Domain.Entities.Users;
using Telegram.Bot.Types;
using Data_ICommand = Picker.Application.Data.ICommand;
using ICommand = Picker.Application.Data.ICommand;

namespace Picker.Persistence.CommandFactory.Commands;

public class CleanerCommand(IColiverRepository coliverRepository) : Data_ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var response = await coliverRepository.GetCleanerToday(date);
        return response;
    }
}