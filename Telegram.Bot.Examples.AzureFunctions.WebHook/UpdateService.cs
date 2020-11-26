using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.AzureFunctions.WebHook
{
    public class UpdateService
    {
        private readonly TelegramBotClient botClient;
        private readonly ILogger<UpdateService> logger;

        public UpdateService()
        {
            var token = Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.Process);

            if (token == null)
            {
                throw new ArgumentException("Can not get token. Set token in environment setting");
            }

            this.botClient = new TelegramBotClient(token);
        }

        public async Task EchoAsync(Update update)
        {
            this.logger?.LogInformation("Invoke telegram update function");

            if (update == null)
                return;

            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                this.logger?.LogInformation("Received Message from {0}", message.Chat.Id);
                await this.botClient.SendTextMessageAsync(message.Chat, $"Echo : {message.Text}");
            }

        }
    }
}
