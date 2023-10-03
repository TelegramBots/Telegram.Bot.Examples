// The worker entry-point
//
// Background service consuming a scoped service.
// See more: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#consuming-a-scoped-service-in-a-background-task
//
// This file simply forms that "link" between the .NET
// class to the actual service functions implemented in
// FSharp.Examples.Polling.Services.Internal.PollingServiceFuncs
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Examples.Polling.Services

open System
open System.Threading
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting

open FSharp.Examples.Polling.Util

type PollingService(sp: IServiceProvider, logger: ILogger<PollingService>) =
  inherit BackgroundService()
  override __.ExecuteAsync(cts: CancellationToken) = task {
    logInfo logger "Starting polling service"
    let receive _ =
      use scope = sp.CreateScope()
      let receiver = scope.ServiceProvider.GetRequiredService<ReceiverService<UpdateHandler>>()
      receiver.ReceiveAsync cts

    let isCancellationRequested _ = cts.IsCancellationRequested

    Seq.initInfinite receive
    |> Seq.takeWhile isCancellationRequested
    |> ignore

    return 0
  }
