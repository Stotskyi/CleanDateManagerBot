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
        ICommandFactory commandFactory) 
    {


        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }


        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            logger.LogInformation("Receive message type: {MessageType}", message.Type);

            if (message.Text is null || !IsRecognizedCommand(message.Text))
            {
                logger.LogInformation("Ignoring unrecognized message: {MessageText}", message?.Text);
                return;
            }

            var chatId = message.Chat.Id;
            var userState = await userStateManager.GetUserStateAsync(chatId) 
                            ?? new UserState { UserId = chatId, State = "start" };

            var response = await HandleUserMessage(userState, message);

            if (response is null) return;

            userState.LastInteraction = DateTime.UtcNow;
            
            await userStateManager.SaveUserStateAsync(userState);
            await botClient.SendTextMessageAsync(chatId, response, cancellationToken: cancellationToken);
        }

        private async Task<string> HandleUserMessage(UserState userState, Message message)
        {
            return userState.State switch
            {
                "awaiting_date" => await HandleAwaitingDateState(userState, message),
                "awaiting_date_to_remove" => await HandleAwaitingDateToRemoveState(userState, message),
                _ => await ExecuteCommandAsync(userState, message)
            };
        }
        private async Task<string> ExecuteCommandAsync(UserState userState, Message message)
        {
            var command = commandFactory.GetCommand(message.Text!);
            if (command != null)
            {
                return await command.Execute(userState, message);
            }
            return "error";
        }
        
        private async Task<string> HandleAwaitingDateState(UserState userState, Message message)
        {
            userState.State = "start";
            var day = message.Text;
            string username = await GetUsername(message);

            var result = await coliverRepository.WriteColiverAsync(day, username);
            return result;
        }

        private async Task<string> HandleAwaitingDateToRemoveState(UserState userState, Message message)
        {
            userState.State = "start";
            var day = message.Text;
            string username = await GetUsername(message);

            var result = await coliverRepository.RemoveFromTable(day, username);
            return result;
        }

        private async Task<string> GetUsername(Message message)
        {
            if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
            {
                return $"@{message.Chat.Username}";
            }
            var member = await botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
            return member.User.Username;
            
        }

        private bool IsRecognizedCommand(string text)
        {
            return text.StartsWith("/enroll")
                   || text.StartsWith("/table")
                   || text.StartsWith("/cleaner")
                   || text.StartsWith("/generateCycle")
                   || text.StartsWith("/remove")
                   || int.TryParse(text, out int day);
        }
        
        public async Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException =>
                    $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        }

        public Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}
