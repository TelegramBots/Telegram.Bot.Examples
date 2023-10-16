// Receiver to receive messages from user
//
// The received messages are also displatched to
// the update handler to get responses to be sent
// back to the user
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.
namespace FSharp.Examples.Polling.Services

open System.Threading
open Microsoft.Extensions.Logging
open Telegram.Bot.Polling
open Telegram.Bot.Types.Enums
open Telegram.Bot

open FSharp.Examples.Polling.Services.Internal

open FSharp.Examples.Polling.Util

type ReceiverService<'T when 'T :> IUpdateHandler>(botClient: ITelegramBotClient, updateHandler: UpdateHandler, logger: ILogger<'T>) =
  interface IReceiverService with
    member __.ReceiveAsync(cts: CancellationToken) = task {

      logInfo logger "ReceiveAsync called"

      let options = ReceiverOptions(
        AllowedUpdates = [||],
        ThrowPendingUpdates = true
       )

      let! me = botClient.GetMeAsync(cts) |> Async.AwaitTask
      let username =
        match me.Username with
        | null -> "My Awesome Bot"
        | v -> v

      logInfo logger $"Start receiving updates for {username}"

      botClient.ReceiveAsync(
        updateHandler = updateHandler,
        receiverOptions = options,
        cancellationToken = cts) |> Async.AwaitTask |> ignore
  }
