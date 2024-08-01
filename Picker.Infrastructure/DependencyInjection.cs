using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Picker.Application.Services;
using Picker.Infrastructure.Extension;
using Telegram.Bot;
using WebApplication2.Models;

namespace Picker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var botConfigurationSection = configuration.GetSection(BotConfiguration.Configuration);
        services.Configure<BotConfiguration>(botConfigurationSection);
        
        var botConfiguration = botConfigurationSection.Get<BotConfiguration>();


        services.AddHostedService<ConfigureWebhook>();
        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                TelegramBotClientOptions options = new(botConfig.BotToken);
                return new TelegramBotClient(options, httpClient);
            });
        
    
        services.AddScoped<UpdateHandlers.UpdateHandlers>();
        return services;
    }
}