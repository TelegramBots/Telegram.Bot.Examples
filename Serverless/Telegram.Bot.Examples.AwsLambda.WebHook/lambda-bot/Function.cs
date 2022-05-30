using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaBot;

public class Function
{
    private static readonly UpdateService updateService;

    static Function()
    {
        updateService = new UpdateService();
    }

    // link it to api gateway / rest / post / lambda function with lambda proxy integration enabled
    // to set it as webhook post manually to https://api.telegram.org/bot<token>/setWebhook
    // with form-data: "url": "<your_invoke_url>"
#pragma warning disable CA1822 // Mark members as static
    public async Task<string> FunctionHandler(JObject request, ILambdaContext context)
#pragma warning restore CA1822 // Mark members as static
    {
        context.Logger.LogInformation("REQUEST: " + JsonConvert.SerializeObject(request));

        try
        {
            Update? updateEvent = request.ToObject<Update>();
            await updateService.EchoAsync(updateEvent);
        }
        catch (Exception e)
        {
            context.Logger.LogError("exception: " + e.Message);
        }

        return "fine from lambda bot " + DateTimeOffset.UtcNow.ToString();
    }
}
