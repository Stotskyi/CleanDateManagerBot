using Microsoft.EntityFrameworkCore;
using OpenAI.Extensions;
using Picker.Application.Services;
using Picker.Infrastructure.Data;
using Picker.Infrastructure.Repository;
using Picker.Infrastructure.Repository.Implementations;
using Picker.Infrastructure.Repository.Interfaces;
using Telegram.Bot;
using WebApplication2;
using WebApplication2.Controllers;
using WebApplication2.Extension;
using WebApplication2.Models;
using WebApplication2.Services;

var builder = WebApplication.CreateBuilder(args);

var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>();
builder.Services.AddDbContext<ApplicationContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<IUserStateRepository,UserStateRepository>();
builder.Services.AddScoped<IColiverRepository,ColiverRepository>();

var dummyConfigurationSection = builder.Configuration.GetSection("Dummy");

builder.Services.AddScoped<UpdateHandlers>();
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
        TelegramBotClientOptions options = new(botConfig.BotToken);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenAIService();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();