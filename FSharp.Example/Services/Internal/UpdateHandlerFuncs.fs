// Implementation of all the Update Handlers
//
// This file contains tha actual F# implementations of all
// handlers, using the idiomatic patterns in F#.
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Example.Services.Internal

open System
open System.IO
open System.Threading

open Microsoft.Extensions.Logging

open Telegram.Bot
open Telegram.Bot.Exceptions
open Telegram.Bot.Types
open Telegram.Bot.Types.Enums
open Telegram.Bot.Types.InlineQueryResults
open Telegram.Bot.Types.ReplyMarkups

open FSharp.Example.Util

module UpdateHandlerFuncs =
  module private BotTextMessages =
    let sendInlineKeyboard (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      botClient.SendChatActionAsync(
        chatId = message.Chat.Id,
        action = ChatAction.Typing,
        cancellationToken = cts) |> Async.AwaitTask |> ignore

      let inlineKeyboard = seq {
        // first row
        seq {
          InlineKeyboardButton.WithCallbackData("1.1", "11");
          InlineKeyboardButton.WithCallbackData("1.2", "12");
        }

        // second row
        seq {
          InlineKeyboardButton.WithCallbackData("2.1", "21");
          InlineKeyboardButton.WithCallbackData("2.2", "22");
        }
      }

      botClient.SendTextMessageAsync(
        chatId = message.Chat.Id,
        text = "Choose",
        replyMarkup = InlineKeyboardMarkup(inlineKeyboard),
        cancellationToken = cts)
      |> Async.AwaitTask |> Async.Ignore

    let sendReplyKeyboard (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      let replyKeyboardMarkup =
        ReplyKeyboardMarkup( seq {
          // first row
          seq { KeyboardButton("1.1"); KeyboardButton("1.2") };

          // second row
          seq { KeyboardButton("1.1"); KeyboardButton("1.2") };
        },
        ResizeKeyboard = true)

      botClient.SendTextMessageAsync(
          chatId = message.Chat.Id,
          text = "Choose",
          replyMarkup = replyKeyboardMarkup,
          cancellationToken = cts)
        |> Async.AwaitTask |> Async.Ignore

    let removeKeyboard (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      botClient.SendTextMessageAsync(
        chatId = message.Chat.Id,
        text = "Removing keyboard",
        replyMarkup = ReplyKeyboardRemove(),
        cancellationToken = cts)
      |> Async.AwaitTask |> Async.Ignore

    let sendFile (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      botClient.SendChatActionAsync(
        message.Chat.Id,
        ChatAction.UploadPhoto,
        cancellationToken = cts)
      |> Async.AwaitTask |> ignore

      let filePath = @"Files/bot.gif"
      let fileName =
        filePath.Split(Path.DirectorySeparatorChar)
        |> Array.last
      let inputStream =
        InputFileStream(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read),
                        fileName)

      botClient.SendPhotoAsync(
        chatId = message.Chat.Id,
        photo = inputStream,
        caption = "Nice Picture",
        cancellationToken = cts)
      |> Async.AwaitTask |> Async.Ignore

    let requestContactAndLocation (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      let requestReplyKeyboard =
        ReplyKeyboardMarkup(seq {
          KeyboardButton.WithRequestLocation("Location");
          KeyboardButton.WithRequestContact("Contact");
        },
        ResizeKeyboard = true)

      botClient.SendTextMessageAsync(
        chatId = message.Chat.Id,
        text = "Who or Where are you?",
        replyMarkup = requestReplyKeyboard,
        cancellationToken = cts)
      |> Async.AwaitTask |> Async.Ignore

    let startInlineQuery (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      let inlineKeyboard =
        InlineKeyboardMarkup (InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"))

      botClient.SendTextMessageAsync(
        chatId = message.Chat.Id,
        text = "Press the button to start Inline Query",
        replyMarkup = inlineKeyboard,
        cancellationToken = cts)
      |> Async.AwaitTask |> Async.Ignore

    let failingHandler (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      raise <| IndexOutOfRangeException()

    let usage (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
      let usage =
        "Usage:\n" +
        "/inline_keyboard - send inline keyboard\n" +
        "/keyboard - send custom keyboard\n" +
        "/remove   - remove custom keyboard\n" +
        "/photo    - send a photo\n" +
        "/request  - request location or contact\n" +
        "/inline_mode - send keyboard with inline query\n" +
        "/throw - Simulate a failed bot message handler\n"

      botClient.SendTextMessageAsync(
        chatId = message.Chat.Id,
        text = usage,
        replyMarkup = ReplyKeyboardRemove(),
        cancellationToken = cts)
      |> Async.AwaitTask |> Async.Ignore

  let botOnMessageReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
    let handleBotMessage f = f botClient logger cts message

    logInfo logger $"Receive message type: {message.Type}"

    match message.Text with
    | text when (not (String.IsNullOrEmpty message.Text)) ->
        // We use tryHead here just in case we get an empty
        // response from the user
        match text.Split(' ') |> Array.tryHead with
        | Some "/inline_keyboard"   -> handleBotMessage BotTextMessages.sendInlineKeyboard
        | Some "/keyboard"          -> handleBotMessage BotTextMessages.sendReplyKeyboard
        | Some "/remove"            -> handleBotMessage BotTextMessages.removeKeyboard
        | Some "/photo"             -> handleBotMessage BotTextMessages.sendFile
        | Some "/request"           -> handleBotMessage BotTextMessages.requestContactAndLocation
        | Some "/inline_mode"       -> handleBotMessage BotTextMessages.startInlineQuery
        | Some "/throw"             -> handleBotMessage BotTextMessages.failingHandler
        | _                         -> handleBotMessage BotTextMessages.usage
    | _ -> async { return () }

  let botOnCallbackQueryReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (query:CallbackQuery) =
    botClient.AnswerCallbackQueryAsync(query.Id, $"Received {query.Data}") |> Async.AwaitTask |> ignore

    botClient.SendTextMessageAsync(
        chatId = query.Message.Chat.Id,
        text = $"Received {query.Data}",
        cancellationToken = cts) |> Async.AwaitTask |> Async.Ignore


  let botOnInlineQueryReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (inlinequery:InlineQuery) =
    logInfo logger $"Received inline query from: {inlinequery.From.Id}"

    // displayed result
    let results = seq {
        InlineQueryResultArticle(
          id = "3",
          title = "TgBots",
          inputMessageContent = InputTextMessageContent("hello"))
      }

    botClient.AnswerInlineQueryAsync(inlinequery.Id,
                                      results |> Seq.cast,
                                      isPersonal = true,
                                      cacheTime = 0,
                                      cancellationToken = cts)
    |> Async.AwaitTask

  let botOnChosenInlineResultReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (chosenInlineResult:ChosenInlineResult) =
    async {
      logInfo logger $"Received inline result: {chosenInlineResult.ResultId}"
    }

  let unknownUpdateHandlerAsync (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (update:Update) =
    async {
      logInfo logger $"Unknown update type: {update.Type}"
    }

  let handleErrorAsync _ (logger: ILogger) _ (err:Exception) = async {
    logInfo logger $"{err.ToString()}"
  }

  let handleUpdateAsync botClient logger cts (update:Update) = async {
    let handleUpdate f  = f botClient logger cts

    try
      let fn =
        match update.Type with
        | UpdateType.Message            -> handleUpdate botOnMessageReceived update.Message
        | UpdateType.EditedMessage      -> handleUpdate botOnMessageReceived update.EditedMessage
        | UpdateType.CallbackQuery      -> handleUpdate botOnCallbackQueryReceived update.CallbackQuery
        | UpdateType.InlineQuery        -> handleUpdate botOnInlineQueryReceived update.InlineQuery
        | UpdateType.ChosenInlineResult -> handleUpdate botOnChosenInlineResultReceived update.ChosenInlineResult
        | _                             -> handleUpdate unknownUpdateHandlerAsync update

      return! fn
    with
      | ex -> do! handleUpdate handleErrorAsync ex
  }
