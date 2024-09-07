using Picker.Application.Abstractions.Messaging;
using Picker.Application.Data;
using Picker.Domain.Entities.Users;
using Telegram.Bot;
using Telegram.Bot.Types;
using ICommand = Picker.Application.Data.ICommand;

namespace Picker.Persistence.CommandFactory.Commands;

public class PokruchCommand : IScheduleCommand
{
    private List<string> Pokruchi = new() {"@St0ks", "@light_blue" ,"@honcharilla","@hatorihandzo","@m5x1m1l3on","@tereza_koss","@Vo1ero","@trojan_o","@sssooniko","@LesykZinchuk","@ptr_khtrn","@maarrggoosshha"};

    public Task<string> Execute()
    {
        return Task.FromResult($"Покидьок пяний і повія драна сьогодні: {Pokruchi[new Random().Next(Pokruchi.Count)]}");
    }
}