using OpenAI.Interfaces;
using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Picker.Application.Commands;

public class GrandFatherCommand(IOpenAIService openAiService) : ICommand
{
    public Task<string> Execute(UserState userState, Message message)
    {
        throw new NotImplementedException();
    }
}