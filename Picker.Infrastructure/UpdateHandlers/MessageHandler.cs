using Microsoft.Extensions.Logging;
using Picker.Application.Services;
using Picker.Domain.Entities.Users;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Picker.Infrastructure.UpdateHandlers;

public class MessageHandler(ILogger<MessageHandler> logger, ITelegramBotClient botClient, IUserStateRepository userStateManager, ICommandFactory commandFactory, IColiverRepository coliverRepository)
    : IMessageHandler
{
    private readonly ITelegramBotClient botClient = botClient;
    private readonly IUserStateRepository userStateManager = userStateManager;
    private readonly ICommandFactory commandFactory = commandFactory;
    private readonly IColiverRepository coliverRepository = coliverRepository;


    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        await BotOnMessageReceived(update.Message!, cancellationToken);
    }
    public bool CanHandle(Update update)
    {
        return update.Message?.Text is null || !IsRecognizedCommand(update.Message.Text);
    }
    
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is null || !IsRecognizedCommand(message.Text))
        {
            return;
        }

        var chatId = message.Chat.Id;
        var userState = await userStateManager.GetUserStateAsync(chatId) 
                        ?? new UserState { UserId = chatId, State = "start" };

        var response = await HandleUserMessage(userState, message,cancellationToken);

        if (response is null) return;

        userState.LastInteraction = DateTime.UtcNow;
            
        await userStateManager.SaveUserStateAsync(userState);
        await botClient.SendTextMessageAsync(chatId, response, cancellationToken: cancellationToken);
    }
    
    private async Task<string> HandleUserMessage(UserState userState, Message message, CancellationToken cancellationToken)
    {
        return userState.State switch
        {
            "awaiting_date" => await HandleAwaitingDateState(userState, message,cancellationToken),
            "awaiting_date_to_remove" => await HandleAwaitingDateToRemoveState(userState, message,cancellationToken),
            _ => await ExecuteCommandAsync(userState, message,cancellationToken)
        };
    }
    
    
     
    private async Task<string> ExecuteCommandAsync(UserState userState, Message message,CancellationToken cancellationToken)
    {
        var command = commandFactory.GetCommand(message.Text!);
        var scheduleCommand = commandFactory.GetScheduleCommand(message.Text!);
        var dickCommand = commandFactory.GetDickCommand(message.Text!);
        var username = await GetUsername(message,cancellationToken);

        if (dickCommand is not null)
        {
            return await dickCommand.Execute(username);
        }
    
        if (scheduleCommand is not null)
        {
            return await scheduleCommand.Execute();
        }

        if (command is not null)
        {
            return await command.Execute(userState, message);
        }

        return "error";
    }
        
    private async Task<string> HandleAwaitingDateState(UserState userState, Message message, CancellationToken cancellationToken)
    {
        userState.State = "start";
        var day = message.Text;
        string username = await GetUsername(message,cancellationToken);

        var result = await coliverRepository.WriteColiverAsync(day, username);
        return result;
    }

    private async Task<string> HandleAwaitingDateToRemoveState(UserState userState, Message message, CancellationToken cancellationToken)
    {
        userState.State = "start";
        var day = message.Text;
        string username = await GetUsername(message,cancellationToken);

        var result = await coliverRepository.RemoveFromTable(day, username);
        return result;
    }

    private async Task<string> GetUsername(Message message, CancellationToken cancellationToken)
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
               || text.StartsWith("/pokruch")
               || text.StartsWith("/dick")
               || text.StartsWith("/stats")
               || int.TryParse(text, out int day);
    }
}

public interface IMessageHandler
{
    public bool CanHandle(Update update);
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}