namespace Telegram.Bot.Examples.WebHook.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}