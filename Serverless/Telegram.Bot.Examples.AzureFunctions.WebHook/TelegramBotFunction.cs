using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.AzureFunctions.WebHook
{
    public static class TelegramBotFunction
    {
        private static readonly UpdateService updateService;

        static TelegramBotFunction()
        {
            updateService = new UpdateService();
        }

        [FunctionName("TelegramBot")]
        public static async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest request,
            ILogger logger)
        {
            try
            {
                var body = await request.ReadAsStringAsync();
                var update = JsonConvert.DeserializeObject<Update>(body);

                await updateService.EchoAsync(update);
            }
            catch (Exception e)
            {
                logger.LogInformation("Exception: " + e.Message);
            }

            return new OkResult();
        }
    }
}
