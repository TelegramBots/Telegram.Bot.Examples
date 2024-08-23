using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);
var token = builder.Configuration["BotToken"]!;             // set your bot token in appsettings.json
var webappUrl = builder.Configuration["BotWebAppUrl"]!;     // set your bot webapp public url in appsettings.json
builder.Services.ConfigureTelegramBot<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt => opt.SerializerOptions);
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Bot entrypoints:
app.MapGet("/bot/setWebhook", async (TelegramBotClient bot) => { await bot.SetWebhookAsync(webappUrl + "/bot"); return $"Webhook set to {webappUrl}/bot"; });
app.MapPost("/bot", OnUpdate);

app.Run();


async void OnUpdate(TelegramBotClient bot, Update update, ILogger<Program> logger)
{
    switch (update)
    {
        case { Message.Text: "/start" }:
            await bot.SendTextMessageAsync(update.Message.Chat, "<b>Let's get started</b>",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: (InlineKeyboardMarkup)InlineKeyboardButton.WithWebApp("Launch Mini-App", webappUrl));
            break;
        default: break;
    }
}
