// Receiver to receive messages from user
//
// The received messages are also displatched to
// the update handler to get responses to be sent
// back to the user
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.
namespace FSharp.Example.Services

open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Telegram.Bot.Polling
open Telegram.Bot.Types
open Telegram.Bot

open FSharp.Example.Services.Internal

open FSharp.Example.Util

type ReceiverService<'T when 'T :> IUpdateHandler>(botClient: ITelegramBotClient, updateHandler: UpdateHandler, logger: ILogger<'T>) =
  interface IReceiverService with
    member __.ReceiveAsync(cts: CancellationToken) = async {

      logInfo logger "ReceiveAsync called"

      let options = ReceiverOptions(
        AllowedUpdates = [||],
        DropPendingUpdates = true
       )

      try
        let! me = botClient.GetMeAsync(cts) |> Async.AwaitTask
        let username =
          match me.Username with
          | null -> "My Awesome Bot"
          | v -> v

        logInfo logger $"Start receiving updates for {username}"

        return! botClient.ReceiveAsync(
          updateHandler = updateHandler,
          receiverOptions = options,
          cancellationToken = cts) |> Async.AwaitTask
        with
        | :? TaskCanceledException -> logInfo logger "INFO: Receive cancelled."
        | e -> logInfo logger $"ERROR: {e.Message}"
  }
