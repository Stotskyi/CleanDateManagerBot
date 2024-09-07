using Picker.Application.Commands;
using Picker.Application.Data;
using Picker.Application.Services;
using Picker.Domain.Entities.Users;
using Picker.Persistence.CommandFactory.Commands;
using Picker.Persistence.Repositories;
using Telegram.Bot;

namespace Picker.Persistence.CommandFactory;

public class CommandFactory(IColiverRepository coliverRepository, ITelegramBotClient botClient,UserService userService) : ICommandFactory
{
    public ICommand? GetCommand(string message) =>
        message switch
        {
            var text when text.StartsWith("/enroll") => new EnrollCommand(),
            var text when text.StartsWith("/remove") => new RemoveCommand(),
            var text when text.StartsWith("/table") => new TableCommand(coliverRepository, botClient),
            var text when text.StartsWith("/generateCycle") => new GenerateCycleCommand(coliverRepository),
            _ => null
        };

    public IScheduleCommand GetScheduleCommand(string message) =>
        message switch
        {
            var text when text is "pokruch" => new PokruchCommand(),
            var text when text.StartsWith("/stats") => new DickRatingCommand(userService),
            var text when text.StartsWith("/cleaner") => new CleanerCommand(coliverRepository),
            _ => null
        };

    public IDickCommand GetDickCommand(string message) =>
        message switch
        {
            var text when text.StartsWith("/dick") => new DickCommand(userService),
            _ => null
        };
    
}
