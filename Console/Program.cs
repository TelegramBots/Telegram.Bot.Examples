using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var token = Environment.GetEnvironmentVariable("TOKEN");
token ??= "{YOUR_BOT_TOKEN}";

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(token, cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
bot.StartReceiving(OnUpdate, OnError);

Console.WriteLine($"@{me.Username} is running... Press Escape to terminate");
while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
cts.Cancel(); // stop the bot

async Task OnError(ITelegramBotClient client, Exception exception, CancellationToken ct)
{
    Console.WriteLine(exception);
    await Task.Delay(2000, ct);
}

async Task OnUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    await (update switch
    {
        { Message: { } message } => OnMessage(message),
        { EditedMessage: { } message } => OnMessage(message, true),
        { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
        _ => OnUnhandledUpdate(update)
    });
}

async Task OnUnhandledUpdate(Update update) => Console.WriteLine($"Received unhandled update {update.Type}");

async Task OnMessage(Message msg, bool edited = false)
{
    if (msg.Text is not { } text)
        Console.WriteLine($"Received a message of type {msg.Type}");
    else if (text.StartsWith('/'))
    {
        var space = text.IndexOf(' ');
        if (space < 0) space = text.Length;
        var command = text[..space].ToLower();
        if (command.LastIndexOf('@') is > 0 and int at) // it's a targeted command
            if (command[(at + 1)..].Equals(me.Username, StringComparison.OrdinalIgnoreCase))
                command = command[..at];
            else
                return; // command was not targeted at me
        await OnCommand(command, text[space..].TrimStart(), msg);
    }
    else
        await OnTextMessage(msg);
}

async Task OnTextMessage(Message msg) // received a text message that is not a command
{
    Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");
    await OnCommand("/start", "", msg); // for now we redirect to command /start
}

async Task OnCommand(string command, string args, Message msg)
{
    Console.WriteLine($"Received command: {command} {args}");
    switch (command)
    {
        case "/start":
            await bot.SendTextMessageAsync(msg.Chat, """
                <b><u>Bot menu</u></b>:
                /photo [url]    - send a photo <i>(optionally from an <a href="https://picsum.photos/310/200.jpg">url</a>)</i>
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                """, parseMode: ParseMode.Html, linkPreviewOptions: true,
                replyMarkup: new ReplyKeyboardRemove()); // also remove keyboard to clean-up things
            break;
        case "/photo":
            if (args.StartsWith("http"))
                await bot.SendPhotoAsync(msg.Chat, args, caption: "Source: " + args);
            else
            {
                await bot.SendChatActionAsync(msg.Chat, ChatAction.UploadPhoto);
                await Task.Delay(2000); // simulate a long task
                await using var fileStream = new FileStream("bot.gif", FileMode.Open, FileAccess.Read);
                await bot.SendPhotoAsync(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
            }
            break;
        case "/inline_buttons":
            List<List<InlineKeyboardButton>> buttons =
            [
                ["1.1", "1.2", "1.3"],
                [
                    InlineKeyboardButton.WithCallbackData("WithCallbackData", "CallbackData"),
                    InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot")
                ],
            ];
            await bot.SendTextMessageAsync(msg.Chat, "Inline buttons:", replyMarkup: new InlineKeyboardMarkup(buttons));
            break;
        case "/keyboard":
            List<List<KeyboardButton>> keys = 
            [
                ["1.1", "1.2", "1.3"],
                ["2.1", "2.2"],
            ];
            await bot.SendTextMessageAsync(msg.Chat, "Keyboard buttons:", replyMarkup: new ReplyKeyboardMarkup(keys) { ResizeKeyboard = true });
            break;
        case "/remove":
            await bot.SendTextMessageAsync(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
            break;
        case "/inline_mode":
            var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
            await bot.SendTextMessageAsync(msg.Chat, "Press the button to start Inline Query\n\n" +
                "(Make sure you enabled Inline Mode in @BotFather)", replyMarkup: new InlineKeyboardMarkup(button));
            break;
    }
}

async Task OnCallbackQuery(CallbackQuery callbackQuery)
{
    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
    await bot.SendTextMessageAsync(callbackQuery.Message!.Chat.Id, $"Received callback from inline button {callbackQuery.Data}");
}
