using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Filters;
using Telegram.Bot.Services;
using Telegram.Bot.Types;

namespace Telegram.Bot.Controllers;

public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] UpdateHandler handleUpdateService,
        CancellationToken cancellationToken)
    {
        try
        {
            await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(exception, Polling.HandleErrorSource.HandleUpdateError, cancellationToken);
        }
        return Ok();
    }
}
