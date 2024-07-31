using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Telegram.Bot.Types;

namespace Picker.Persistence.CommandFactory.Commands;

public class GenerateCycleCommand(IColiverRepository coliverRepository) : ICommand
{
    public async Task<string> Execute(UserState userState, Message message)
    {
        var count = ExtractNumber(message.Text!);
        if (count is 0) return "Цифри навчись писати";
        
        var response = await coliverRepository.CreateCycle(count);
        return response.ToString();
    }

    private byte ExtractNumber(string input)
    {
        try
        {
            var parts = input.Split(" ");
            if (parts.Length > 1 && byte.TryParse(parts[1], out byte count))
                return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting number: {ex.Message}");
        }
    
        return 0;
    }

}