// Receiver Service - receives messages from the user
//
// This file simply forms that "link" between the .NET
// class to the actual receiver functions implemented in
// FSharp.Examples.Polling.Services.Internal.ReceiverFuncs
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Examples.Polling.Services

open System.Threading
open Microsoft.Extensions.Logging
open Telegram.Bot

open FSharp.Examples.Polling.Services.Internal

type ReceiverService<'T>(botClient: ITelegramBotClient, updateHandler: UpdateHandler, logger: ILogger<'T>) =
  member __.ReceiveAsync(cts: CancellationToken) = ReceiverFuncs.receiveAsync botClient logger cts updateHandler
