using Microsoft.Extensions.Logging;
using OpenAI.Interfaces;
using Picker.Application.Abstractions;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Picker.Application.Services
{
    public class UpdateHandlers(
        ITelegramBotClient botClient,
        ILogger<UpdateHandlers> logger,
        IColiverRepository coliverRepository,
        IUserStateRepository userStateManager,
        CommandFactory commandFactory)
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

            if (message.Text is null || !IsRecognizedCommand(message.Text) || !int.TryParse(message.Text, out int number) || number < 1 || number > 31)
            {
                logger.LogInformation("Ignoring unrecognized message: {MessageText}", message?.Text);
                return;
            }

            var chatId = message.Chat.Id;
            var userState = await userStateManager.GetUserStateAsync(chatId) ?? new UserState { UserId = chatId, State = "start" };

            string response = await HandleUserMessage(userState, message);

            if (response is null) return;

            userState.LastInteraction = DateTime.UtcNow;
            await userStateManager.SaveUserStateAsync(userState);

            await botClient.SendTextMessageAsync(chatId, response, cancellationToken: cancellationToken);
        }

        private async Task<string> HandleUserMessage(UserState userState, Message message)
        {
            var command = commandFactory.GetCommand(message.Text!);
            if (command != null)
            {
                return await command.Execute(userState, message);
            }
            
            if (userState.State == "awaiting_date")
            {
                return await HandleAwaitingDateState(userState, message);
            }

            if (userState.State == "awaiting_date_to_remove")
            {
                return await HandleAwaitingDateToRemoveState(userState, message);
            }

            return null;
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
            else
            {
                var member = await botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
                return member.User.Username;
            }
        }

        private bool IsRecognizedCommand(string text)
        {
            return text.StartsWith("/enroll")
                   || text.StartsWith("/table")
                   || text.StartsWith("/cleaner")
                   || text.StartsWith("/generateCycle")
                   || text.StartsWith("/time")
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
