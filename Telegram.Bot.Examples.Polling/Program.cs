using Telegram.Bot.Extensions.Polling;

namespace Telegram.Bot.Examples.Polling;

public static class Program
{
    private static TelegramBotClient? Bot;

    public static async Task Main()
    {
        Bot = new TelegramBotClient(Configuration.BotToken);

        var me = await Bot.GetMeAsync();
        Console.Title = me.Username;

        using var cts = new CancellationTokenSource();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        Bot.StartReceiving(new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
                           cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();
    }
}
