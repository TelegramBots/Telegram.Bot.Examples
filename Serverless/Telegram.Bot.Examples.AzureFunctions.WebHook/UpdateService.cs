using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.AzureFunctions.WebHook
{
    public class UpdateService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(ITelegramBotClient botClient, ILogger<UpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            _logger.LogInformation("Invoke telegram update function");

            if (update is null)
                return;

            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                _logger.LogInformation("Received Message from {0}", message.Chat.Id);
                await _botClient.SendTextMessageAsync(message.Chat, $"Echo : {message.Text}");
            }

        }
    }
}
