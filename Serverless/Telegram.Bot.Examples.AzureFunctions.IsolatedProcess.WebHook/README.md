# Azure Functions Example

## Requirements
- [.NET SDK 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Visual Studio 2022 with Azure development workload
- An Azure account with an active subscription
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)


## Test bot locally

### Set a token
At first, you have to set your own token in the environment variable. For this, input the bot token after command:

```
func settings add TELEGRAM_BOT_TOKEN
```

it will create `local.settings.json` with encrypted token

### Ngrok
Install ngrock from this page [ngrok - download](https://ngrok.com/download).
Run your function. You might need to enable a firewall exception so that the tools can handle HTTP requests.

and start ngrok on port 7071.

```
ngrok http 7071 
```

### Set Webhook
From ngrok you get an URL to your local server. It’s important to use the `https` one. You can post this url as form-data (key: url, value: https://yoursubdomain.ngrok.io/api/TelegramBot) to the telegram api.
https://api.telegram.org/botYourBotToken/setWebhook
Be aware of the **bot** prefix in front of your bot token in the URL.

```
curl --request POST --url https://api.telegram.org/bot<YOUR_TELEGRAM_TOKEN>/setWebhook --header 'content-type: application/json' --data '{"url": "<YOUR_FUNCTION_URL>"}'
```
Now your bot should answer with the text from every message you send to it.


## Deploy

### Visual Studio
Some information is taken from the [official documentation](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio).

1. In Solution Explorer, right-click the project and select Publish
2. In Target, select Azure
3. In Specific target, select Azure Function App (Windows)
4. In Function Instance, select Create a new Azure Function and then use the values specified
5. Select Finish, and on the Publish page, select Publish to deploy the package containing your project files to your new function app in Azure.
6. Open your function on https://portal.azure.com/
7. Select tab **Configuration** and **Add new application setting**, where you enter Name: token, Value: `<BotToken>` and save updates.

### Azure Functions Core Tools
Information is taken from the [official documentation](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)

1. Create function app on https://portal.azure.com/
2. Copy name from the Function App home page or you can get it from JSON view
3. Replace `<function-app-name>`
4. Run command
    ```
    func azure functionapp publish <function-app-name> --publish-local-settings
    ```
    `--publish-local-settings` will publish your token from `local.settings.json`

<br /> **Don't forget to update yours Webhook**