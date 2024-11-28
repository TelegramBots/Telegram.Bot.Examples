using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.AzureFunctions.Webhook;

public class UpdateService(ITelegramBotClient botClient, ILogger<UpdateService> logger)
{
    public async Task EchoAsync(Update update)
    {
        logger.LogInformation("Invoked telegram update function");

        if (update is not { Message: { } message } ) return;

        logger.LogInformation("Received Message from {0}", message.Chat.Id);
        await botClient.SendMessage(message.Chat, $"Echo : {message.Text}");
    }
}
