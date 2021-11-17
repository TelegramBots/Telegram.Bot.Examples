
# ASP.NET Core Webhook Example

## About

This project is a simple ASP.NET Core application, which provides webhook endpoint for the Telegram Bot.

You can find useful information on setting up webhook for your bot in official docs:

- [Marvin's Marvellous Guide to All Things Webhook](https://core.telegram.org/bots/webhooks)
- [Getting updates](https://core.telegram.org/bots/api#getting-updates)
- [setWebhook](https://core.telegram.org/bots/api#setwebhook)
- [getWebhookInfo](https://core.telegram.org/bots/api#getwebhookinfo)

## Setup

Please make sure you have .NET 6 or newer installed. You can download .NET runtime from the [official site.](https://dotnet.microsoft.com/download)
This is a short description how you can test your bot locally. The description presumes that you already have a bot and it’s token. If not, please create one. You’ll find several explanations on the internet how to do this.

### Bot configuration

You have to specify your Bot token in **appsettings.json**. Replace **{BotToken}** in **appsettings.json** with actual Bot token. Also you have to specify endpoint, to which Telegram will send new updates with `HostAddress` parameter:

```json
"BotConfiguration": {
    "BotToken": "{BotToken}",
    "HostAddress": "https://mydomain.com"
}
```

you can specify separate development configuration with **appsettings.Development.json**.

## Ngrok

Ngrok gives you the opportunity to access your local machine from a temporary subdomain provided by ngrok. This domain can later send to the telegram API as URL for the webhook.
Install ngrok from this page: [ngrok - download](https://ngrok.com/download) or via homebrew cask:

```shell
brew install --cask ngrok
```

and start ngrok on port 8443.

```shell
ngrok http 8443 
```

Telegram API only supports the ports 443, 80, 88 or 8443. Feel free to change the port in the config of the project.

### Set Webhook

From ngrok you get an URL to your local server. It’s important to use the `https` one. You can manually set webhook with  [setWebhook](https://core.telegram.org/bots/api#setwebhook) API call, providing this URL as form-data (key: url, value: `https://yoursubdomain.ngrok.io/api/update`).

### Run Bot

Now you can start the Bot in a local instance. Check if the port of the application matches the port on which ngrok is running.

Now your bot should answer with the text from every message you send to it.
