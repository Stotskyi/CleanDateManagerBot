using Picker.Application.Interfaces;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot.Types;

namespace Picker.Application.Commands;

public class CleanerCommand(IColiverRepository coliverRepository) : ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var response = await coliverRepository.GetCleanerToday(date);
        return response;
    }
}