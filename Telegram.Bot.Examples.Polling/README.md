# Telegram.Bot Polling Example

## About

This example demonstrates simple Telegram Bot using long polling 
([wiki](https://en.wikipedia.org/wiki/Push_technology#Long_polling)).

This example utilize [Worker Service](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers)
template for hosting Bot application. This approach gives you such benefits as:

- cross-platform hosting;
- configuration;
- dependency injection (DI);
- logging;


## Prerequisites

Please make sure you have .NET 6 or newer installed. You can download .NET runtime from the [official site.](https://dotnet.microsoft.com/download)

You have to add [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) 
nuget package to your project to be able to use polling:

```shell
dotnet add package Telegram.Bot
```

Make sure that your .csproj contains these items (versions may vary):

```xml
<ItemGroup>
  <PackageReference Include="Telegram.Bot" Version="18.0.0" />
</ItemGroup>
```

## Configuration

You should provide your Telegram Bot token with one of the available providers.
Read futher on [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0).
We provide plaecehoder for bot configuration in `appsettings*.json`. You have to replace {BOT_TOKEN} with actual Bot token:

```json
"BotConfiguration": {
  "BotToken": "{BOT_TOKEN}"
}
```

Watch [Configuration in .NET 6](https://www.youtube.com/watch?v=6Fg54CEBVno&t=170s) talk for deep dive into .NET Configuration.


## Run Bot As Windows Service

Follow [Create a Windows Service using `BackgroundService`](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service)
article to host your bot as a Windows Service.

## Run Bot Daemon On Linux

Follow [.NET Core and systemd](https://devblogs.microsoft.com/dotnet/net-core-and-systemd/) blog post to run your
bot as a Linux `systemd` daemon.
