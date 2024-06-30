using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Examples.AzureFunctions.IsolatedProcess.Webhook;

var tgToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.Process)
    ?? throw new ArgumentException("Can not get token. Set token in environment setting");

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        s.AddHttpClient("tgclient")
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(tgToken, httpClient));

        // Dummy business-logic service
        s.AddScoped<UpdateService>();
    })
    .Build();

host.Run();
