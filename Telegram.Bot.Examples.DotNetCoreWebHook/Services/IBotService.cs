using System.Collections.Generic;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}