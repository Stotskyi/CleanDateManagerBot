using Picker.Application.Interfaces;
using Picker.Infrastructure.Entities;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Picker.Application.Commands;

public class TableCommand(IColiverRepository coliverRepository, ITelegramBotClient botClient) : ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var imageData = await coliverRepository.GetCleanersTable();
        
        await botClient.SendChatActionAsync(message.Chat, ChatAction.UploadPhoto);

        await using var memoryStream = new MemoryStream(imageData);
        await botClient.SendPhotoAsync(message.Chat, new InputFileStream(memoryStream));

        return null;
    }
}