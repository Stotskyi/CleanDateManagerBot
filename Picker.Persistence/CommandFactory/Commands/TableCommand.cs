using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Picker.Application.Commands;

public class TableCommand(IColiverRepository coliverRepository, ITelegramBotClient botClient) : ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var imageData = await coliverRepository.GetCleanersTable();

        if (imageData is null) return "німа";
        await botClient.SendChatActionAsync(message.Chat, ChatAction.UploadPhoto);

        await using var memoryStream = new MemoryStream(imageData);
        await botClient.SendPhotoAsync(message.Chat, new InputFileStream(memoryStream));

        return null;
    }
}