// Bot entrypoint
//
// Copyright (c) 2023 Arvind Devarajan
// Licensed to you under the MIT License.
// See the LICENSE file in the project root for more information.

namespace FSharp.Example

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open Telegram.Bot

open FSharp.Example.Services

type ReceiverSvc = ReceiverService<UpdateHandler>

type BotConfiguration() =
  member val BotToken: string

module Program =
  let createHostBuilder args =
    Host.CreateDefaultBuilder(args)
        .ConfigureServices(fun context services ->

      let cfg = context.Configuration.GetSection(nameof(BotConfiguration))
      let token = cfg.["BotToken"]

      // Register named HttpClient to benefits from IHttpClientFactory
      // and consume it with ITelegramBotClient typed client.
      // More read:
      //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
      //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
      services.AddHttpClient("telegram_bot_client").AddTypedClient<ITelegramBotClient>(
          fun httpClient sp ->
            let options = TelegramBotClientOptions(token)
            TelegramBotClient(options, httpClient) :> ITelegramBotClient
        ) |> ignore

      services.AddScoped<UpdateHandler>() |> ignore
      services.AddScoped<ReceiverSvc>() |> ignore
      services.AddHostedService<PollingService<ReceiverSvc>>() |> ignore)

  [<EntryPoint>]
  let main args =
    createHostBuilder(args).Build().Run()

    0 // exit code
