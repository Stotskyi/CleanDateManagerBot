using OpenAI.Extensions;
using Picker.Application;
using Picker.Infrastructure;
using Picker.Infrastructure.Extension;
using Picker.Persistence;
using WebApplication2.Controllers;
using WebApplication2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);
var botConfiguration = botConfigurationSection.Get<BotConfiguration>();


builder.Services.AddControllers().AddNewtonsoftJson();

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