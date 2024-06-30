using Amazon.Lambda.Core;
using System.Text.Json;
using Telegram.Bot.Serialization;
using Telegram.Bot.Types;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaBot;

public class LambdaFunction
{
    private static readonly UpdateService updateService = new();

    // link it to api gateway / rest / post / lambda function with lambda proxy integration enabled
    // to set it as webhook post manually to https://api.telegram.org/bot<token>/setWebhook
    // with form-data: "url": "<your_invoke_url>"
#pragma warning disable CA1822 // Mark members as static
    public async Task<string> FunctionHandler(JsonElement request, ILambdaContext context)
#pragma warning restore CA1822 // Mark members as static
    {
        context.Logger.LogInformation("REQUEST: " + JsonSerializer.Serialize(request));

        try
        {
            Update? updateEvent = request.Deserialize<Update>(JsonSerializerOptionsProvider.Options);
            if (updateEvent is null)
            {
                const string resultMessage = "Unable to deserialize Update.";
                context.Logger.LogWarning(resultMessage);
                return resultMessage;
            }

            await updateService.EchoAsync(updateEvent);
        }
#pragma warning disable CA1031
        catch (Exception e)
#pragma warning restore CA1031
        {
            context.Logger.LogError("exception: " + e.Message);
        }

        return FormattableString.Invariant($"fine from lambda bot {DateTimeOffset.UtcNow}");
    }
}
