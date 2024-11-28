using Amazon.Lambda.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace LambdaBot;

public class UpdateService
{
    private readonly TelegramBotClient botClient = new("<token>"); // replace with your bot token

    public async Task EchoAsync(Update update)
    {
        if (update is not { Message: { } message }) return;

        LambdaLogger.Log("Received Message from " + message.Chat.Id);
        switch (message.Type)
        {
            case MessageType.Text: // Let's echo text messages
                await botClient.SendMessage(message.Chat.Id, message.Text!);
                break;

            case MessageType.Photo: // Let's download the photo
                File file = await botClient.GetFile(message.Photo![^1].FileId);

                string filename = file.FileId + Path.GetExtension(file.FilePath);
                await using (FileStream saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await botClient.DownloadFile(file.FilePath!, saveImageStream);
                }

                await botClient.SendMessage(message.Chat.Id, "Thx for the Pics");
                break;
        }
    }
}
