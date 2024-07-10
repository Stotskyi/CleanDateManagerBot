using Picker.Application.Commands;
using Picker.Application.Interfaces;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Picker.Application.Abstractions;

public class CommandFactory(IColiverRepository coliverRepository, ITelegramBotClient botClient)
{
    public ICommand GetCommand(string messageText) =>
        messageText switch
        {
            var text when text == "/enroll" => new EnrollCommand(),
            var text when text == "/remove" => new RemoveCommand(),
            var text when text == "/table" => new TableCommand(coliverRepository, botClient),
            var text when text == "/cleaner" => new CleanerCommand(coliverRepository),
            var text when text == "/time" => new TimeCommand(coliverRepository),
            var text when text == "/generateCycle" => new GenerateCycleCommand(coliverRepository)
        };
}
