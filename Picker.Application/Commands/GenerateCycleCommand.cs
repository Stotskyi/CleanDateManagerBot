using Picker.Application.Interfaces;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot.Types;

namespace Picker.Application.Commands;

public class GenerateCycleCommand(IColiverRepository coliverRepository) : ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var response = await coliverRepository.CreateCycle(11);
        return response.ToString();
    }
}