using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.AzureFunctions.WebHook;

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
            var update = JsonConvert.DeserializeObject<Update>(body);

            await _updateService.EchoAsync(update);
        }
        catch (Exception e)
        {
            logger.LogInformation("Exception: " + e.Message);
        }

        return new OkResult();
    }
}
