
# .NET Core Web Hook Example
## About
This example provides an telegram bot, based on the .NET Core framework. Feel free to use it as scaffold for your own bot projects.
## Setup
This is an short description how you can test your bot locally. The description presumes that you already have a bot and it’s token. If not, please create one. You’ll find several explanations on the internet how to do this.

### Replace token
At first you have to set your own token in the **appsettings.json**. For this, replace **<BotToken>** in the **appsettings.json** with the token that belongs to your bot.
```
"BotConfiguration": {
    "BotToken": "<BotToken>"    
  }
```
In **appsettings.Development.json** you can provide the token of a dev bot, if you like to have two separated. If not, you have to remove the **BotToken** section.

### Ngrok
Ngrok gives you the opportunity to access your local machine from a temporary subdomain provided by ngrok. This domain can later send to the telegram API as URL for the webhook.
Install ngrock from this page [ngrok - download](https://ngrok.com/download) or via homebrew cask 
```
brew cask install ngrok
```
and start ngrok on port 8443.
```
ngrok http 8443 
```
Telegram API only supports the ports 443, 80, 88 or 8443. Feel free to change the port in the config of the project.

### Set Webhook
From ngrok you get an URL to your local server. It’s important to use the `https` one. You can post this url als form-data (key: url, value: https://yoursubdomain.ngrok.io/api/update) to the telegram api.
https://api.telegram.org/botYourBotToken/setWebhook
Be aware of the **bot** prefix in front of your bot token in the URL.

###  Run bot local
Now you can start the Bot in a local instance. Check if the port of the application matches the port on which ngrok is running.

Now your bot should answer with the text from every message you send to it.
