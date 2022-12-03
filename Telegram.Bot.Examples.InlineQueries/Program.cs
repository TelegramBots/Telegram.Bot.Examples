using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

const int MaxEntries = 20;

var token = Environment.GetEnvironmentVariable("TOKEN")!;

// load all articles, sorted alphabetically, in memory (not optimal if you have a huge base of articles)
var files = Directory.GetFiles("Articles", "*.html");
Array.Sort(files, StringComparer.CurrentCultureIgnoreCase);
var articleNames = new string[files.Length];
var articleContents = new string[files.Length];
for (int i = 0; i < files.Length; i++)
{
    articleNames[i] = Path.GetFileNameWithoutExtension(files[i]);
    articleContents[i] = System.IO.File.ReadAllText(files[i]);
}

// start the bot
var bot = new TelegramBotClient(token);
var me = await bot.GetMeAsync();
using var cts = new CancellationTokenSource();
bot.StartReceiving(HandleUpdateAsync, PollingErrorHandler, null, cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// stop the bot
cts.Cancel();

Task PollingErrorHandler(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine($"Exception while polling for updates: {ex}");
    return Task.CompletedTask;
}

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    try
    {
        await (update.Type switch
        {
            UpdateType.InlineQuery => BotOnInlineQueryReceived(bot, update.InlineQuery!),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(bot, update.ChosenInlineResult!),
            _ => Task.CompletedTask
        });
    }
#pragma warning disable CA1031
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while handling {update.Type}: {ex}");
    }
#pragma warning restore CA1031
}

// for this method to be called, you need to enable "Inline mode" on in BotFather
async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
{
    var results = new List<InlineQueryResult>();

    // find and return articles that starts with the keyword
    var index = Array.BinarySearch(articleNames, inlineQuery.Query, StringComparer.CurrentCultureIgnoreCase);
    if (index < 0) index = ~index;

    while (index < articleNames.Length && results.Count < MaxEntries &&
            articleNames[index].StartsWith(inlineQuery.Query, StringComparison.CurrentCultureIgnoreCase))
    {
        results.Add(
            new InlineQueryResultArticle($"{index}",
                articleNames[index],
                new InputTextMessageContent(articleContents[index])
                {
                    ParseMode = ParseMode.Html // content is in HTML formatting
                })
            {
                Description = Regex.Replace(articleContents[index], "<.*?>", "") // results will show beginning of article contents (without HTML tags)
            });
        index++;
    }
    await botClient.AnswerInlineQueryAsync(inlineQuery.Id, results);
}

// for this method to be called, you need to enable "Inline feedback" in BotFather (100% if you want to know all your users choices)
Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
{
    if (uint.TryParse(chosenInlineResult.ResultId, out uint index) && index < articleNames.Length)
    {
        Console.WriteLine($"User {chosenInlineResult.From} has selected article: {articleNames[index]}");
    }
    return Task.CompletedTask;
}
