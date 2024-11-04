using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

const int MaxEntries = 20;

// replace YOUR_BOT_TOKEN below, or set your TOKEN in Project Properties > Debug > Launch profiles UI > Environment variables
var token = Environment.GetEnvironmentVariable("TOKEN") ?? "YOUR_BOT_TOKEN";

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
using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(token, cancellationToken: cts.Token);
var me = await bot.GetMe();
bot.OnUpdate += OnUpdate;

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();
cts.Cancel(); // stop the bot

async Task OnUpdate(Update update)
{
    try
    {
        switch (update.Type)
        {
            case UpdateType.InlineQuery: await OnInlineQuery(bot, update.InlineQuery!); break;
            case UpdateType.ChosenInlineResult: await OnChosenInlineResult(bot, update.ChosenInlineResult!); break;
        };
    }
#pragma warning disable CA1031
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while handling {update.Type}: {ex}");
    }
#pragma warning restore CA1031
}

// for this method to be called, you need to enable "Inline mode" on in BotFather
async Task OnInlineQuery(ITelegramBotClient botClient, InlineQuery inlineQuery)
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
    await botClient.AnswerInlineQuery(inlineQuery.Id, results);
}

// for this method to be called, you need to enable "Inline feedback" in BotFather (100% if you want to know all your users choices)
async Task OnChosenInlineResult(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
{
    if (uint.TryParse(chosenInlineResult.ResultId, out uint index) && index < articleNames.Length)
    {
        Console.WriteLine($"User {chosenInlineResult.From} has selected article: {articleNames[index]}");
    }
}
