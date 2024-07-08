using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Picker.Infrastructure.Data;
using Telegram.Bot.Types;
using VilnyyBot.Services;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;


public class BotController(ApplicationContext context,IOptions<Dummy> options) : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, CancellationToken cancellationToken)
    {
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }

    [HttpGet("string")]
    public async Task<string> GetTestString()
    {
        return "i am working";
    }
    
    [HttpGet("test")]
    public async Task<string> GetTest()
    {
        return options.Value.test;
    }

    [HttpGet("GetDatabase")]
    public async Task<OkObjectResult> GetInfoFromDatabase()
    {
        return Ok(await context.Colivers.ToListAsync());
    }
}

public class Dummy
{
    public string test { get; set; }
}