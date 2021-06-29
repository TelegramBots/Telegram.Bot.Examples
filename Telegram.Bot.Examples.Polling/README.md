# Telegram.Bot Polling Example

## About

This example demonstrates simple Telegram Bot built with [Telegram.Bot.Extensions.Polling](https://github.com/TelegramBots/Telegram.Bot.Extensions.Polling) library.

## Prerequisites

You have to add nuget packages to your project to be able to use polling:

- Add [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) with a package manager in your IDE or from command line:

  ```shell
  dotnet add package Telegram.Bot --version 16.0.0
  ```

- Add [Telegram.Bot.Extensions.Polling](https://www.nuget.org/packages/Telegram.Bot.Extensions.Polling/) with a package manager in your IDE or from command line:

  ```shell
  dotnet add package Telegram.Bot.Extensions.Polling --version 0.2.0
  ```

Make sure that your .csproj contains these items (versions may vary):

```xml
<ItemGroup>
  <PackageReference Include="Telegram.Bot" Version="16.0.0" />
  <PackageReference Include="Telegram.Bot.Extensions.Polling" Version="0.2.0" />
</ItemGroup>
```
