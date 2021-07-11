// Example Telegram bot in F#
//
// Copyright (c) 2021 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace Telegram.Bot.Extensions.Polling.Fsharp

open System
open System.IO
open System.Threading
open Telegram.Bot
open Telegram.Bot.Types
open Telegram.Bot.Exceptions
open Telegram.Bot.Extensions.Polling
open Telegram.Bot.Types.Enums
open Telegram.Bot.Types.InlineQueryResults
open Telegram.Bot.Types.InputFiles
open Telegram.Bot.Types.ReplyMarkups

module TelegramBot =
    [<EntryPoint>]
    let main argv =

        let botClient
            = TelegramBotClient(TelegramBotCfg.token)

        let handleErrorAsync (bot:ITelegramBotClient) (err:Exception) (cts:CancellationToken) = async {
            let errormsg =
                match err with
                | :? ApiRequestException as apiex -> $"Telegram API Error:\n[{apiex.ErrorCode}]\n{apiex.Message}"
                | _                               -> err.ToString()

            Console.WriteLine(errormsg)
        }

        let botOnCallbackQueryReceived (query:CallbackQuery) = async {
            do!
                botClient.AnswerCallbackQueryAsync(query.Id, $"Received {query.Data}")
                |> Async.AwaitTask

            do!
                botClient.SendTextMessageAsync(ChatId(query.Message.Chat.Id), $"Received {query.Data}")
                |> Async.AwaitTask
                |> Async.Ignore
        }

        let botOnInlineQueryReceived (inlinequery:InlineQuery) = async {
            Console.WriteLine($"Received inline query from: {inlinequery.From.Id}");

            // displayed result
            let results = seq {
                InlineQueryResultArticle(
                  id = "3",
                  title = "TgBots",
                  inputMessageContent = InputTextMessageContent("hello"))
            }

            do!
                botClient.AnswerInlineQueryAsync(inlinequery.Id,
                                                 results |> Seq.cast,
                                                 isPersonal = true,
                                                 cacheTime = 0)
                |> Async.AwaitTask
                |> Async.Ignore
        }

        let botOnChosenInlineResultReceived (chosenInlineResult:ChosenInlineResult) = async {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}")
        }

        let botOnMessageReceived (message:Message) =
            Console.WriteLine($"Receive message type: {message.Type}");

            let sendInlineKeyboard = async {
                do!
                    botClient.SendChatActionAsync(ChatId(message.Chat.Id), ChatAction.Typing)
                    |> Async.AwaitTask
                    |> Async.Ignore

                let inlineKeyboard = seq {
                    // first row
                    seq {
                        InlineKeyboardButton.WithCallbackData("1.1", "11");
                        InlineKeyboardButton.WithCallbackData("1.2", "12");
                    };

                    // second row
                    seq {
                        InlineKeyboardButton.WithCallbackData("2.1", "21");
                        InlineKeyboardButton.WithCallbackData("2.2", "22");
                    };
                }

                do!
                    botClient.SendTextMessageAsync(
                        chatId = ChatId(message.Chat.Id),
                        text = "Choose",
                        replyMarkup = InlineKeyboardMarkup(inlineKeyboard))
                    |> Async.AwaitTask
                    |> Async.Ignore
            }

            let sendReplyKeyboard = async {
                let replyKeyboardMarkup =
                    ReplyKeyboardMarkup( seq {

                        // first row
                        seq { KeyboardButton("1.1"); KeyboardButton("1.2") };

                        // second row
                        seq { KeyboardButton("1.1"); KeyboardButton("1.2") };
                    })

                do!
                    botClient.SendTextMessageAsync(
                        chatId = ChatId(message.Chat.Id),
                        text = "Choose",
                        replyMarkup = replyKeyboardMarkup)
                    |> Async.AwaitTask
                    |> Async.Ignore
            }

            let removeKeyboard = async {
                do!
                    botClient.SendTextMessageAsync(
                        chatId = ChatId(message.Chat.Id),
                        text = "Removing keyboard",
                        replyMarkup = ReplyKeyboardRemove())
                    |> Async.AwaitTask
                    |> Async.Ignore
            }

            let sendFile = async {
                do!
                    botClient.SendChatActionAsync(ChatId(message.Chat.Id), ChatAction.UploadPhoto)
                    |> Async.AwaitTask
                    |> Async.Ignore

                let filePath = @"Files/tux.png"
                use fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

                let fileName =
                    filePath.Split(Path.DirectorySeparatorChar)
                    |> Array.last

                do!
                    botClient.SendPhotoAsync(
                        chatId = ChatId(message.Chat.Id),
                        photo = InputOnlineFile(fileStream, fileName),
                        caption = "Nice Picture")
                    |> Async.AwaitTask
                    |> Async.Ignore
            }

            let requestContactAndLocation = async {
                let requestReplyKeyboard =
                    ReplyKeyboardMarkup(seq {
                        KeyboardButton.WithRequestLocation("Location");
                        KeyboardButton.WithRequestContact("Contact");
                    })

                do!
                    botClient.SendTextMessageAsync(
                        chatId = ChatId(message.Chat.Id),
                        text = "Who or Where are you?",
                        replyMarkup = requestReplyKeyboard)
                    |> Async.AwaitTask
                    |> Async.Ignore
            }

            let usage = async {
                let usage =
                    "Usage:\n" +
                    "/inline   - send inline keyboard\n" +
                    "/keyboard - send custom keyboard\n" +
                    "/remove   - remove custom keyboard\n" +
                    "/photo    - send a photo\n" +
                    "/request  - request location or contact"

                do!
                    botClient.SendTextMessageAsync(
                        chatId = ChatId(message.Chat.Id),
                        text = usage,
                        replyMarkup = ReplyKeyboardRemove())
                    |> Async.AwaitTask
                    |> Async.Ignore
            }

            async {
                if message.Type <> MessageType.Text then
                    ()
                else
                    let fn =
                        // We use tryHead here just in case we get an empty
                        // response from the user
                        match message.Text.Split(' ') |> Array.tryHead with
                        | Some "/inline"   -> sendInlineKeyboard
                        | Some "/keyboard" -> sendReplyKeyboard
                        | Some "/remove"   -> removeKeyboard
                        | Some "/photo"    -> sendFile
                        | Some "/request"  -> requestContactAndLocation
                        | _                -> usage

                    do! fn
            }

        let unknownUpdateHandlerAsync (update:Update) = async {
            Console.WriteLine($"Unknown update type: {update.Type}");
        }

        let handleUpdateAsync bot (update:Update) cts = async {
            try
                let fn =
                    match update.Type with
                    | UpdateType.Message            -> botOnMessageReceived update.Message
                    | UpdateType.EditedMessage      -> botOnMessageReceived update.EditedMessage
                    | UpdateType.CallbackQuery      -> botOnCallbackQueryReceived update.CallbackQuery
                    | UpdateType.InlineQuery        -> botOnInlineQueryReceived update.InlineQuery
                    | UpdateType.ChosenInlineResult -> botOnChosenInlineResultReceived update.ChosenInlineResult
                    | _                             -> unknownUpdateHandlerAsync update
                fn |> Async.Start
            with
                | ex -> do! handleErrorAsync bot ex cts
        }

        async {
            let! me = botClient.GetMeAsync() |> Async.AwaitTask
            // Console.Title = me.Username |> ignore
            printfn $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            printfn $"Start listening for {me.Username}..."

            use cts = new CancellationTokenSource();

            // There is quite some bit of jugglery here, so requires some explanation:
            // DefaultUpdateHandler() requires two arguments, both of type Func<_,_,_,_>, and both
            // of which return a Task. Now, in order to be in the F# domain, we would like to
            // have this Func<> defined as an F# function: and so we need to explicitely construct
            // Func<>s with by passing them an inner lambda function.
            // Now, the inner lambda function needs to return a Task, but we want to use F# async.
            // We therefore use a Async.StartAsTask to start an async computation and get back a Task.
            // The last ":>" is for upcasting Task<unit> (returned by the async computations) to their
            // base class Task to avoid and error that says "the expression expects a Task but we have a
            // Task<unit> here.". Since F# can already infer the base-type, we simply upcast to "_".
            // The good thing about doing all this is that handleUpdateAsync and handleErrorAsync are both
            // in the F# domain - so now can take all advantages of F#!
            botClient.StartReceiving(DefaultUpdateHandler(
                                        Func<_,_,_,_>(fun b u t ->  Async.StartAsTask (handleUpdateAsync b u t) :> _),
                                        Func<_,_,_,_>(fun b e t ->  Async.StartAsTask (handleErrorAsync b e t) :> _)),
                                        cts.Token)
        } |> Async.Start

        printfn "Press any key to exit"
        Console.Read() |> ignore
        0
