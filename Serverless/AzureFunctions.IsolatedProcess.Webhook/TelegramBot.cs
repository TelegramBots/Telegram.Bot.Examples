using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Telegram.Bot.Types;

#pragma warning disable CA1031 // Do not catch general exception types

namespace Telegram.Bot.Examples.AzureFunctions.IsolatedProcess.Webhook;

public class TelegramBot(ILogger<TelegramBot> logger, UpdateService updateService)
{
    [Function("TelegramBot")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        var response = request.CreateResponse(HttpStatusCode.OK);
        try
        {
            var body = await request.ReadAsStringAsync() ?? throw new ArgumentNullException(nameof(request));
            var update = JsonSerializer.Deserialize<Update>(body, JsonBotAPI.Options);
            if (update is null)
            {
                logger.LogWarning("Unable to deserialize Update object.");
                return response;
            }

            await updateService.EchoAsync(update);
        }
        catch (Exception e)
        {
            logger.LogError("Exception: {Message}", e.Message);
        }
        return response;
    }
}
