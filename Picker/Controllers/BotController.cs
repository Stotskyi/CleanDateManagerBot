using Microsoft.AspNetCore.Mvc;
using Picker.Infrastructure.UpdateHandlers;
using Telegram.Bot.Types;
using WebApplication2.Models;

namespace WebApplication2.Controllers;


public class BotController() : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, CancellationToken cancellationToken)
    {
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
