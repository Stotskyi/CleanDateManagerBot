using Microsoft.Extensions.Logging;
using Picker.Application.Services;
using Picker.Domain.Entities.Users;
using Picker.Persistence.CommandFactory;
using Picker.Persistence.Repositories;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Picker.Infrastructure.UpdateHandlers
{
    public class UpdateHandlers(
        ITelegramBotClient botClient,
        ILogger<UpdateHandlers> logger,
        IColiverRepository coliverRepository,
        IUserStateRepository userStateManager,
        ICommandFactory commandFactory,
        IMessageHandler messageHandler)
    {
      
        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            logger.LogInformation("Received update type: {UpdateType}", update.Type);
            
            
            var handler = update switch
            {
                { Message: { } message } => messageHandler.HandleUpdateAsync(update, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }
        public Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}
