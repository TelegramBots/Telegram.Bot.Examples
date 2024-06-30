using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Telegram.Bot.Serialization;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.AzureFunctions.Webhook;

public class TelegramBotFunction
{
    private readonly UpdateService _updateService;

    public TelegramBotFunction(UpdateService updateService)
    {
        _updateService = updateService;
    }

    [FunctionName("TelegramBot")]
    public async Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest request,
        ILogger logger)
    {
        try
        {
            var body = await request.ReadAsStringAsync();
            var update = JsonSerializer.Deserialize<Update>(body, JsonSerializerOptionsProvider.Options);
            if (update is null)
            {
                logger.LogWarning("Unable to deserialize Update object.");
                return new OkResult();
            }

            await _updateService.EchoAsync(update);
        }
#pragma warning disable CA1031
        catch (Exception e)
#pragma warning restore CA1031
        {
            logger.LogError("Exception: " + e.Message);
        }

        return new OkResult();
    }
}
