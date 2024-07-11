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
            var text when text.StartsWith("/enroll") => new EnrollCommand(),
            var text when text.StartsWith("/remove") => new RemoveCommand(),
            var text when text.StartsWith("/table") => new TableCommand(coliverRepository, botClient),
            var text when text.StartsWith("/cleaner") => new CleanerCommand(coliverRepository),
            var text when text.StartsWith("/time") => new TimeCommand(coliverRepository),
            var text when text.StartsWith("/generateCycle") => new GenerateCycleCommand(coliverRepository)
        };
}
