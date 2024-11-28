using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Telegram.Bot.Types;

#pragma warning disable CA1031

namespace Telegram.Bot.Examples.AzureFunctions.Webhook;

public class TelegramBotFunction(ILogger<TelegramBotFunction> logger, UpdateService updateService)
{
    [FunctionName("TelegramBot")]
    public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request)
    {
        try
        {
            var body = await request.ReadAsStringAsync();
            var update = JsonSerializer.Deserialize<Update>(body, JsonBotAPI.Options);
            if (update is null)
            {
                logger.LogWarning("Unable to deserialize Update object.");
                return new OkResult();
            }

            await updateService.EchoAsync(update);
        }
        catch (Exception e)
        {
            logger.LogError("Exception: " + e.Message);
        }
        return new OkResult();
    }
}
