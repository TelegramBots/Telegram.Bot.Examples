using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
