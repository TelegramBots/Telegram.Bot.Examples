using System;
using System.IO;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public class UpdateService : IUpdateService
    {
        readonly IBotService _botService;

        public UpdateService(IBotService botService){
            _botService = botService;
        }

        public void Echo(Update update)
        {
            var message = update.Message;

            Console.WriteLine("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.TextMessage)
            {
                // Echo each Message
                _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text).GetAwaiter();

            }
            else if (message.Type == MessageType.PhotoMessage)
            {
                // Download Photo
                var file = _botService.Client.GetFileAsync(message.Photo.LastOrDefault()?.FileId).Result;

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    file.FileStream.CopyToAsync(saveImageStream);
                }

                _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }
        }
    }
}
