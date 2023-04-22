using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Telegram.Bot.Examples.AzureFunctions.IsolatedProcess.WebHook
{
    public class TelegramBot
    {
        private readonly ILogger _logger;

        public TelegramBot(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TelegramBot>();
        }

        [Function("TelegramBot")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
