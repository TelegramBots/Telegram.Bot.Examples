// Implementation of all the Update Handlers
//
// This file contains tha actual F# implementations of all
// handlers, using the idiomatic patterns in F#.
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Examples.Polling.Services.Internal

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

open FSharp.Examples.Polling.Util

module UpdateHandlerFuncs =
  let handlePollingErrorAsync _ (logger: ILogger) _ (err:Exception) = task {
    let errormsg =
      match err with
      | :? ApiRequestException as apiex -> $"Telegram API Error:\n[{apiex.ErrorCode}]\n{apiex.Message}"
      | _                                                     -> err.ToString()

    logInfo logger $"{errormsg}"
  }

  let botOnCallbackQueryReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (query:CallbackQuery) = async {
    do!
      botClient.AnswerCallbackQueryAsync(query.Id, $"Received {query.Data}")
      |> Async.AwaitTask

    do!
      botClient.SendTextMessageAsync(
        chatId = query.Message.Chat.Id,
        text = $"Received {query.Data}",
        cancellationToken = cts)
      |> Async.AwaitTask
      |> Async.Ignore
  }

  let botOnInlineQueryReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (inlinequery:InlineQuery) = async {
    logInfo logger $"Received inline query from: {inlinequery.From.Id}"

    do!
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
      |> Async.Ignore
  }

  let botOnChosenInlineResultReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (chosenInlineResult:ChosenInlineResult) = async {
    logInfo logger $"Received inline result: {chosenInlineResult.ResultId}"
  }

  let botOnMessageReceived (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (message:Message) =
    logInfo logger $"Receive message type: {message.Type}"

    let sendInlineKeyboard = async {
      do!
        botClient.SendChatActionAsync(
          chatId = message.Chat.Id,
          chatAction = ChatAction.Typing,
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore

      // Simulate a long running task
      async { do! Async.Sleep 500  } |> ignore

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
          chatId = message.Chat.Id,
          text = "Choose",
          replyMarkup = InlineKeyboardMarkup(inlineKeyboard),
          cancellationToken = cts)
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
        },
        ResizeKeyboard = true)

      do!
        botClient.SendTextMessageAsync(
          chatId = message.Chat.Id,
          text = "Choose",
          replyMarkup = replyKeyboardMarkup,
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore
    }

    let removeKeyboard = async {
      do!
        botClient.SendTextMessageAsync(
          chatId = message.Chat.Id,
          text = "Removing keyboard",
          replyMarkup = ReplyKeyboardRemove(),
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore
    }

    let sendFile = async {
      do!
        botClient.SendChatActionAsync(
          message.Chat.Id,
          ChatAction.UploadPhoto,
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore

      let filePath = @"Files/tux.png"
      use fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)

      let fileName =
        filePath.Split(Path.DirectorySeparatorChar)
        |> Array.last

      do!
        botClient.SendPhotoAsync(
          chatId = message.Chat.Id,
          photo = InputFileStream(fileStream, fileName),
          caption = "Nice Picture",
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore
    }

    let requestContactAndLocation = async {
      let requestReplyKeyboard =
        ReplyKeyboardMarkup(seq {
          KeyboardButton.WithRequestLocation("Location");
          KeyboardButton.WithRequestContact("Contact");
        },
        ResizeKeyboard = true)

      do!
        botClient.SendTextMessageAsync(
          chatId = message.Chat.Id,
          text = "Who or Where are you?",
          replyMarkup = requestReplyKeyboard,
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore
    }

    let startInlineQuery = async {
      let inlineKeyboard =
        InlineKeyboardMarkup (InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"))

      do!
        botClient.SendTextMessageAsync(
          chatId = message.Chat.Id,
          text = "Press the button to start Inline Query",
          replyMarkup = inlineKeyboard,
          cancellationToken = cts)
        |> Async.AwaitTask
        |> Async.Ignore
    }

    let failingHandler =
      raise <| IndexOutOfRangeException()

    let usage = async {
      let usage =
        "Usage:\n" +
        "/inline_keyboard - send inline keyboard\n" +
        "/keyboard - send custom keyboard\n" +
        "/remove   - remove custom keyboard\n" +
        "/photo    - send a photo\n" +
        "/request  - request location or contact" +
        "/inline_mode - send keyboard with inline query" +
        "/throw - Simulate a failed bot message handler"

      do!
        botClient.SendTextMessageAsync(
          chatId = message.Chat.Id,
          text = usage,
          replyMarkup = ReplyKeyboardRemove(),
          cancellationToken = cts)
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
          | Some "/inline_keyboard"   -> sendInlineKeyboard
          | Some "/keyboard"          -> sendReplyKeyboard
          | Some "/remove"            -> removeKeyboard
          | Some "/photo"             -> sendFile
          | Some "/request"           -> requestContactAndLocation
          | Some "/inline_mode"       -> startInlineQuery
          | Some "/throw"             -> failingHandler
          | _                         -> usage

        do! fn
    }

  let unknownUpdateHandlerAsync (botClient:ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (update:Update) = async {
    logInfo logger $"Unknown update type: {update.Type}"
  }

  let handleUpdateAsync botClient logger cts (update:Update) = task {
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
      return fn |> Async.Start
    with
      | ex -> do! handleUpdate handlePollingErrorAsync ex
  }
