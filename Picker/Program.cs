using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Extensions.Logging.AzureAppServices;
using OpenAI.Extensions;
using Picker.Application;
using Picker.Infrastructure;
using Picker.Infrastructure.Extension;
using Picker.Persistence;
using WebApplication2.Controllers;
using WebApplication2.Models;
using BasicAuthAuthorizationFilter = Hangfire.Dashboard.BasicAuthorization.BasicAuthAuthorizationFilter;
using BasicAuthAuthorizationFilterOptions = Hangfire.Dashboard.BasicAuthorization.BasicAuthAuthorizationFilterOptions;
using BasicAuthAuthorizationUser = Hangfire.Dashboard.BasicAuthorization.BasicAuthAuthorizationUser;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "logs-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});
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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] {new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions()
    {
        RequireSsl = true,
        SslRedirect = false,
        LoginCaseSensitive = true,
        Users = new []
        {
            new BasicAuthAuthorizationUser()
            {
                Login = "admin",  
                PasswordClear = "admin"  
            }
        }
    })}
});
app.UseHangfireServer(config =>
{   
    config.UseServer(2);
});


app.Run();
