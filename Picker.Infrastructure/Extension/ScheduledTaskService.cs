using System.Security.Cryptography;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Picker.Application.Services;
using Telegram.Bot;

namespace Picker.Infrastructure.Extension;

public class ScheduledTaskService(
    ILogger<ScheduledTaskService> logger,
    IServiceProvider serviceProvider) : IHostedService
{
    public async Task PokrychOfDay()
    {
        using var scope = serviceProvider.CreateScope();
        var commandFactory = scope.ServiceProvider.GetRequiredService<ICommandFactory>();
        
        using var scope1 = serviceProvider.CreateScope();
        var botClient = scope1.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        
        var job = commandFactory.GetScheduleCommand("pokruch");
        var message = await job.Execute();
        await botClient.SendTextMessageAsync(-1001807080149,message);
    }
    
    public async Task Cleaner()
    {
        using var scope = serviceProvider.CreateScope();
        var commandFactory = scope.ServiceProvider.GetRequiredService<ICommandFactory>();
        
        using var scope1 = serviceProvider.CreateScope();
        var botClient = scope1.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        
        var job = commandFactory.GetScheduleCommand("/cleaner");
        var message = await job.Execute();
        await botClient.SendTextMessageAsync(-1001807080149,message);
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start work");
        RecurringJob.AddOrUpdate("Cleaner", () => Cleaner(), "0 9 * * *");
        RecurringJob.AddOrUpdate("Pokrych", () => PokrychOfDay(), "0 9 * * *");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}