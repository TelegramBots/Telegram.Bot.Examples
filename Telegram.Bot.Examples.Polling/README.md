# Telegram.Bot Polling Example

## About

This example demonstrates simple Telegram Bot built with [Telegram.Bot.Extensions.Polling](https://github.com/TelegramBots/Telegram.Bot.Extensions.Polling) library.

## Prerequisites

Please make sure you have .NET 6 or newer installed. You can download .NET runtime from the [official site.](https://dotnet.microsoft.com/download)

You have to add nuget packages to your project to be able to use polling:

- Add [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) with a package manager in your IDE or from command line:

  ```shell
  dotnet add package Telegram.Bot
  ```

- Add [Telegram.Bot.Extensions.Polling](https://www.nuget.org/packages/Telegram.Bot.Extensions.Polling/) with a package manager in your IDE or from command line:

  ```shell
  dotnet add package Telegram.Bot.Extensions.Polling
  ```

Make sure that your .csproj contains these items (versions may vary):

```xml
<ItemGroup>
  <PackageReference Include="Telegram.Bot" Version="17.0.0" />
  <PackageReference Include="Telegram.Bot.Extensions.Polling" Version="1.0.0" />
</ItemGroup>
```
