using System;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaBot
{
    public class Function
    {
        private static readonly AmazonLambdaClient lambdaClient;
        private static readonly UpdateService updateService;

        static Function()
        {
            lambdaClient = new AmazonLambdaClient();
            updateService = new UpdateService();
        }

        // link it to api gateway / rest / post / lambda function with lambda proxy integration enabled
        // to set it as webhook post manually to https://api.telegram.org/bot<token>/setWebhook
        // with form-data: "url": "<your_invoke_url>"
        public async Task<string> FunctionHandler(JObject request, ILambdaContext context)
        {
            LambdaLogger.Log("REQUEST: " + JsonConvert.SerializeObject(request));

            try
            {
                var updateEvent = request.ToObject<Update>();
                await updateService.EchoAsync(updateEvent);
            }
            catch (Exception e)
            {
                LambdaLogger.Log("exception: " + e.Message);
            }

            return "fine from lambda bot " + DateTimeOffset.UtcNow.ToString();
        }
    }
}
