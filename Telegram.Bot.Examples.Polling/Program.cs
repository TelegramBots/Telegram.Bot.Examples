using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.Polling;

public static class Program
{
    private static TelegramBotClient? Bot;

    public static async Task Main()
    {
        Bot = new TelegramBotClient(Configuration.BotToken);

        User me = await Bot.GetMeAsync();
        Console.Title = me.Username ?? "My awesome Bot";

        using var cts = new CancellationTokenSource();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        Bot.StartReceiving(updateHandler: Handlers.HandleUpdateAsync,
                           errorHandler: Handlers.HandleErrorAsync,
                           receiverOptions: new ReceiverOptions()
                           {
                               AllowedUpdates = Array.Empty<UpdateType>()
                           },
                           cancellationToken: cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();
    }
}
