using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.AzureFunctions.IsolatedProcess.Webhook;

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

        if (!(update.Message is { } message)) return;

        _logger.LogInformation("Received Message from {ChatId}", message.Chat.Id);
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"Echo : {message.Text}");
    }
}
