using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json;
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

var provider = new FileExtensionContentTypeProvider() { Mappings = { [".tgs"] = "application/x-tgsticker" } };
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Bot entrypoints:
app.MapGet("/bot/setWebhook", async (TelegramBotClient bot) => { await bot.SetWebhookAsync(webappUrl + "/bot"); return $"Webhook set to {webappUrl}/bot"; });
app.MapPost("/bot", OnUpdate);
// WebAppDemo & DurgerKing backend: (their AJAX don't use XSRF antiforgery but we validate Telegram Data anyway)
app.MapPost("/cafe/api", Cafe.OnCafeApi).DisableAntiforgery();
app.MapPost("/demo/api", OnDemoApi).DisableAntiforgery();

app.Run();


async void OnUpdate(TelegramBotClient bot, Update update, ILogger<Program> logger)
{
    switch (update)
    {
        case { Message.Text: "/start" }:
            await bot.SendTextMessageAsync(update.Message.Chat, "<b>Let's get started</b>\n\nTry our Razor Webapp or order your perfect lunch! 🍟",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: (InlineKeyboardMarkup)InlineKeyboardButton.WithWebApp("Launch Mini-App", webappUrl));
            break;
        case { PreCheckoutQuery: { } pcq }:
            await bot.AnswerPreCheckoutQueryAsync(pcq.Id, Cafe.OnPreCheckoutQuery(pcq));
            break;
        case { Message.SuccessfulPayment: { } sp }:
            await bot.SendTextMessageAsync(update.Message.Chat, "Thank you for your 'payment'! Don't worry, your imaginary credit card was not charged. Your order is not on the way.");
            break;
        case { Message.WebAppData: { } wad }:
            logger.LogInformation("Received WebAppData: {Data} | button {ButtonText}", wad.Data, wad.ButtonText);
            break;
        default: break;
    }
}

async Task<object> OnDemoApi(TelegramBotClient bot, IConfiguration config, [FromForm] string method, IFormCollection form)
{
    var query = AuthHelpers.ParseValidateData(form["_auth"], config["BotToken"]!);
    switch (method)
    {
        case "checkInitData":
            return new { ok = true };
        case "sendMessage":
            string? msg_id = form["msg_id"], with_webview = form["with_webview"];
            var user = JsonSerializer.Deserialize<User>(query["user"], JsonBotAPI.Options)!;
            await bot.SendTextMessageAsync(user.Id, "Hello, World!",
                replyMarkup: with_webview == "0" ? new ReplyKeyboardRemove() :
                    new ReplyKeyboardMarkup(true).AddButton(KeyboardButton.WithWebApp("Open WebApp", webappUrl + "/demo")));
            return new { response = new { ok = true } };
        default:
            return new { ok = false, error = "Unsupported method: " + method };
    }
}
