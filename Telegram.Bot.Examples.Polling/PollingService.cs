using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.Polling;

public class PollingService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UpdateHandlers _updateHandlers;
    private readonly ILogger<PollingService> _logger;

    public PollingService(
        ITelegramBotClient botClient,
        UpdateHandlers updateHandlers,
        ILogger<PollingService> logger)
    {
        _botClient = botClient;
        _updateHandlers = updateHandlers;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var me = await _botClient.GetMeAsync(stoppingToken);
                _logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "My Awesome Bot");

                // Start receiving updates
                await _botClient.ReceiveAsync(
                    updateHandler: _updateHandlers.HandleUpdateAsync,
                    pollingErrorHandler: _updateHandlers.PollingErrorHandler,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError ("Polling failed with exception: {Exception}", ex);

                // Cool down if something goes wrong
                await Task.Delay(2_0000, stoppingToken);
            }
        }
    }
}
