// Receiver to receive messages from user
//
// The received messages are also displatched to
// the update handler to get responses to be sent
// back to the user
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Examples.Polling.Services.Internal

open System.Threading.Tasks
open System.Threading

open Microsoft.Extensions.Logging

open Telegram.Bot.Polling
open Telegram.Bot.Types.Enums
open Telegram.Bot

// Type of Receiver Async function
type ReceiverAsyncFunc = CancellationToken -> Task

module ReceiverFuncs =
  let receiveAsync (botClient: ITelegramBotClient) (logger: ILogger) (cts: CancellationToken) (uh: IUpdateHandler) = task {
    let options = ReceiverOptions(
      AllowedUpdates = Array.empty<UpdateType>,
      ThrowPendingUpdates = true
    )

    let! me = botClient.GetMeAsync(cts)
    let username =
      match me.Username with
      | null -> "My Awesome Bot"
      | v -> v

    logger.LogInformation $"Start receiving updates for {username}"

    botClient.ReceiveAsync(
      updateHandler = uh,
      receiverOptions = options,
      cancellationToken = cts
    ) |> ignore
  }
