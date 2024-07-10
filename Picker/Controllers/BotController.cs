using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Picker.Application.Services;
using Picker.Infrastructure.Data;
using Telegram.Bot.Types;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers;


public class BotController(ApplicationContext context,IOptions<BotConfiguration> options) : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, CancellationToken cancellationToken)
    {
        if (update.Message.Text == "HELOO") return Ok("get out of fuck");
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }

    [HttpGet("string")]
    public async Task<string> GetTestString()
    {
        return "i am working";
    }
    
    [HttpGet("test")]
    public async Task<OkObjectResult> GetTest()
    {
        return Ok(options);
    }

    [HttpGet("GetDatabase")]
    public async Task<OkObjectResult> GetInfoFromDatabase()
    {
        return Ok(await context.Colivers.ToListAsync());
    }
}
