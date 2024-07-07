using OpenAI.Interfaces;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace VilnyyBot.Services
{
    public class UpdateHandlers(ITelegramBotClient botClient, ILogger<UpdateHandlers> logger, IOpenAIService openAiService,
        IColiverRepository coliverRepository, IUserStateRepository userStateManager)
    {
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
            var userState = await userStateManager.GetUserStateAsync(chatId) ?? new UserState { UserId = chatId, State = "start" };

            string response = await HandleUserMessage(userState, message);
            
            if (response is null) return;
            
            userState.LastInteraction = DateTime.UtcNow;
            await userStateManager.SaveUserStateAsync(userState);
            
            await botClient.SendTextMessageAsync(chatId, response, cancellationToken: cancellationToken);
        }

        private async Task<string> HandleUserMessage(UserState userState, Message message)
        {
            if (message.Text.StartsWith("/enroll"))
            {
                userState.State = "awaiting_date";
                return "Видави день";
            }
            
            if (message.Text.StartsWith("/remove"))
            {
                userState.State = "awaiting_date_to_remove";
                return "Видави день";
            }
            

            if (message.Text.StartsWith("/table"))
            {
                var imageData = await coliverRepository.GetCleanersTable();
                string filePath = "Files/bot.gif";
                await File.WriteAllBytesAsync(filePath, imageData);
                await botClient.SendChatActionAsync(message.Chat, ChatAction.UploadPhoto);
                
                await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
                await botClient.SendPhotoAsync(message.Chat, new InputFileStream(fileStream));
            }
            
            if (message.Text.StartsWith("/cleaner"))
            {
                var date = DateOnly.FromDateTime(DateTime.Now);
                var response = await coliverRepository.GetCleanerToday(date);
                return response.ToString();
            }

            if (message.Text.StartsWith("/generateCycle"))
            {
                var response = await coliverRepository.CreateCycle(11); //await coliverRepository.GetCleanerToday(date);
                return response.ToString();
            }
            if (message.Text.StartsWith("/time"))
            {
                var response = await coliverRepository.GetRangeOfDate(); //await coliverRepository.GetCleanerToday(date);
                return response;
            }

            if (userState.State == "awaiting_date")
            {
                userState.State = "start";

                var day = message.Text;
                string username;
                if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                {
                    username = $"@{message.Chat.Username}";
                }
                else
                {
                    var member = await botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
                    username = member.User.Username;
                }
                var result =  await coliverRepository.WriteColiverAsync(day, username);
                return result;
            }
            
            if (userState.State == "awaiting_date_to_remove")
            {
                userState.State = "start";

                var day = message.Text;
                string username;
                if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                {
                    username = $"@{message.Chat.Username}";
                }
                else
                {
                    var member = await botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id);
                    username = member.User.Username;
                }
                var result =  await coliverRepository.RemoveFromTable(day, username);
                return result;
            }
            
            
            return null;
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
    }

}
