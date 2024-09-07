using System.Windows.Input;
using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Telegram.Bot.Types;
using Data_ICommand = Picker.Application.Data.ICommand;
using ICommand = Picker.Application.Data.ICommand;

namespace Picker.Persistence.CommandFactory.Commands;

public class CleanerCommand(IColiverRepository coliverRepository) : IScheduleCommand
{
    public async Task<string> Execute()
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var response = await coliverRepository.GetCleanerToday(date);
        return response;
    }
    
}