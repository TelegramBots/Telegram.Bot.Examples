using Amazon.Lambda.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace LambdaBot;

public class UpdateService
{
    private readonly TelegramBotClient botClient;

    public UpdateService()
    {
        // replace with your bot token
        botClient = new TelegramBotClient("<token>");
    }

    public async Task EchoAsync(Update update)
    {
        if (update is null)
        {
            return;
        }

        if (!(update.Message is { } message))
        {
            return;
        }

        LambdaLogger.Log("Received Message from " + message.Chat.Id);

        switch (message.Type)
        {
            case MessageType.Text:
                // Echo each Message
                await botClient.SendTextMessageAsync(message.Chat.Id, message.Text!);
                break;

            case MessageType.Photo:
                // Download Photo
                string fileId = message.Photo![^1].FileId;
                File file = await botClient.GetFileAsync(fileId);

                string filename = file.FileId + "." + file.FilePath!.Split('.').Last();
                await using (FileStream saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await botClient.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
                break;
        }
    }
}
