using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Telegram.Bot.Serialization;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.AzureFunctions.IsolatedProcess.Webhook;

public class TelegramBot
{
    private readonly ILogger<TelegramBot> _logger;
    private readonly UpdateService _updateService;

    public TelegramBot(ILogger<TelegramBot> logger, UpdateService updateService)
    {
        _logger = logger;
        _updateService = updateService;
    }

    [Function("TelegramBot")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var response = request.CreateResponse(HttpStatusCode.OK);
        try
        {
            var body = await request.ReadAsStringAsync() ?? throw new ArgumentNullException(nameof(request));
            var update = JsonSerializer.Deserialize<Update>(body, JsonSerializerOptionsProvider.Options);
            if (update is null)
            {
                _logger.LogWarning("Unable to deserialize Update object.");
                return response;
            }

            await _updateService.EchoAsync(update);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            _logger.LogError("Exception: {Message}", e.Message);
        }

        return response;
    }
}
