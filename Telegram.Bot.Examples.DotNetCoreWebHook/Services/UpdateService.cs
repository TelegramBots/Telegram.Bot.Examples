using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger)
        {
            _botService = botService;
            _logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.Text)
            {
                // Echo each Message
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var fileId = message.Photo.LastOrDefault()?.FileId;
                var file = await _botService.Client.GetFileAsync(fileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await _botService.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }
        }
    }
}
