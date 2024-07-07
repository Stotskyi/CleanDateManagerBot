using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Picker.Infrastructure.Data;
using Telegram.Bot.Types;
using VilnyyBot.Services;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;


public class BotController(ApplicationContext context) : ControllerBase
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

    [HttpGet("GetDatabase")]
    public async Task<OkObjectResult> GetInfoFromDatabase()
    {
        return Ok(await context.Colivers.ToListAsync());
    }
}