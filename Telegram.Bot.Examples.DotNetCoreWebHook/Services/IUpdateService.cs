using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public interface IUpdateService
    {
        void Echo(Update update);
    }
}
