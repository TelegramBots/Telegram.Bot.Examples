using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Filters;
using Telegram.Bot.Types;
using Webhook.Controllers.Services;

namespace Webhook.Controllers.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(IOptions<BotConfiguration> Config) : ControllerBase
{
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook([FromServices] TelegramBotClient bot, CancellationToken ct)
    {
        var webhookUrl = Config.Value.BotWebhookUrl.AbsoluteUri;
        await bot.SetWebhookAsync(webhookUrl, allowedUpdates: [], secretToken: Config.Value.SecretToken, cancellationToken: ct);
        return $"Webhook set to {webhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
            return Forbid();
        try
        {
            await handleUpdateService.HandleUpdateAsync(update, ct);
        }
        catch (Exception exception)
        {
            await handleUpdateService.HandleErrorAsync(exception, ct);
        }
        return Ok();
    }
}
