using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace Telegram.Bot.Examples.WebHook
{
    public static class Bot
    {
        public static readonly TelegramBotClient Api = new TelegramBotClient("Your API Key");
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            // Endpoint must be configured with netsh:
            // netsh http add urlacl url=https://+:8443/ user=<username>
            // netsh http add sslcert ipport=0.0.0.0:8443 certhash=<cert thumbprint> appid=<random guid>

            using (WebApp.Start<Startup>("https://+:8443"))
            {
                // Register WebHook
                // You should replace {YourHostname} with your Internet accessible hosname
                Bot.Api.SetWebhookAsync("https://{YourHostname}:8443/WebHook").Wait();

                Console.WriteLine("Server Started");

                // Stop Server after <Enter>
                Console.ReadLine();

                // Unregister WebHook
                Bot.Api.DeleteWebhookAsync().Wait();
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            configuration.Routes.MapHttpRoute("WebHook", "{controller}");

            app.UseWebApi(configuration);
        }
    }

    public class WebHookController : ApiController
    {
        public async Task<IHttpActionResult> Post(Update update)
        {
            var message = update.Message;

            Console.WriteLine("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.Text)
            {
                // Echo each Message
                await Bot.Api.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var file = await Bot.Api.GetFileAsync(message.Photo.LastOrDefault()?.FileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = File.Open(filename, FileMode.Create))
                {
                    await Bot.Api.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await Bot.Api.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }

            return Ok();
        }
    }
}
