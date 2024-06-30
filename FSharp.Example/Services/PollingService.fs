// The worker entry-point
//
// Background service consuming a scoped service.
// See more: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#consuming-a-scoped-service-in-a-background-task
//
// This file simply forms that "link" between the .NET
// class to the actual service functions implemented in
// FSharp.Example.Services.Internal.PollingServiceFuncs
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Example.Services

open System
open System.Threading

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting

open FSharp.Example.Services.Internal
open FSharp.Example.Util

type PollingService<'T when 'T:> IReceiverService>(sp: IServiceProvider, logger: ILogger<PollingService<'T>>) =
  inherit BackgroundService()

  member __.doWork (cts: CancellationToken) = task {
    let getReceiverService _ =
      use scope = sp.CreateScope()
      let service: 'T = scope.ServiceProvider.GetRequiredService<'T>()
      service

    let cancellationNotRequested _ = (not cts.IsCancellationRequested)

    try
      return Seq.initInfinite getReceiverService
      |> Seq.takeWhile cancellationNotRequested
      |> Seq.iter (fun r -> (r.ReceiveAsync cts |> Async.RunSynchronously |> ignore))
    with
    | e ->
        logger.LogError($"Polling failed with exception: {e.ToString}: {e.Message}");
  }

  override __.ExecuteAsync(cts: CancellationToken) =
    logInfo logger "Starting polling service"
    __.doWork cts
