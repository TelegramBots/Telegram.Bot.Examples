using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LambdaBot
{
    public class UpdateService
    {
        private readonly TelegramBotClient botClient;
        private readonly ILogger<UpdateService> logger;

        public UpdateService()
        {
            // replace with your bot token
            this.botClient = new TelegramBotClient("<token>");
        }

        public async Task EchoAsync(Update update)
        {
            if (update == null)
            {
                return;
            }

            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            this.logger?.LogInformation("Received Message from {0}", message.Chat.Id);

            switch (message.Type)
            {
                case MessageType.Text:
                    // Echo each Message
                    await this.botClient.SendTextMessageAsync(message.Chat.Id, message.Text);
                    break;

                case MessageType.Photo:
                    // Download Photo
                    var fileId = message.Photo.LastOrDefault()?.FileId;
                    var file = await this.botClient.GetFileAsync(fileId);

                    var filename = file.FileId + "." + file.FilePath.Split('.').Last();
                    using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                    {
                        await this.botClient.DownloadFileAsync(file.FilePath, saveImageStream);
                    }

                    await this.botClient.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
                    break;
            }
        }
    }
}
