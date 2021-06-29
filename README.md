# Telegram.Bot Examples

## About

This branch contains sample applications based on [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library:

- [Classic ASP .NET MVC application](https://github.com/TelegramBots/Telegram.Bot.Examples/tree/legacy-ASPNET).

## Prerequisites

Endpoint must be configured with netsh:

```shell
netsh http add urlacl url=https://+:8443/ user=<username>
netsh http add sslcert ipport=0.0.0.0:8443 certhash=<cert thumbprint> appid=<random guid>
```

You can find more about `netsh http` commands in this article: [Netsh http commands](https://docs.microsoft.com/en-us/windows-server/networking/technologies/netsh/netsh-http)

## Requirements

This example require .NET Framework 4.5.2.

## Community

Feel free do join our [group chat](https://t.me/tgbots_dotnet)!
