using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Telegram.Bot.Examples.AzureFunctions.Webhook.Startup))]

namespace Telegram.Bot.Examples.AzureFunctions.Webhook;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Register ILogger<T> and ILoggerFactory
        builder.Services.AddLogging();

        var token = Environment
            .GetEnvironmentVariable("token", EnvironmentVariableTarget.Process)
            ?? throw new ArgumentException("Can not get token. Set token in environment setting");

        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        builder.Services.AddHttpClient("tgclient")
            .AddTypedClient<ITelegramBotClient>(httpClient
                => new TelegramBotClient(token, httpClient));

        // Dummy business-logic service
        builder.Services.AddScoped<UpdateService>();
    }
}
