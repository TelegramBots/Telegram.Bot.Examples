using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Examples.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Setup Bot configuration
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        services.AddScoped<UpdateHandlers>();

        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                TelegramBotClientOptions options = new(botConfig.BotToken);
                return new TelegramBotClient(options, httpClient);
            });
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();

public class BotConfiguration
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; set; } = "";
}

public static class ServiceProviderExtension
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T: class
    {
        var o = serviceProvider.GetService<IOptions<T>>();
        if (o is null)
            throw new ArgumentNullException(nameof(T));

        return o.Value;
    }
}
