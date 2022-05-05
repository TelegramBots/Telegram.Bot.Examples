using Telegram.Bot;
using Telegram.Bot.Examples.Polling;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

var bot = new TelegramBotClient(Configuration.BotToken);

var me = await bot.GetMeAsync();
Console.Title = me.Username ?? "My awesome Bot";

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
bot.StartReceiving(updateHandler: Handlers.HandleUpdateAsync,
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
