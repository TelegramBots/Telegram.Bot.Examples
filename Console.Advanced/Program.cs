using Microsoft.Extensions.Options;
using Telegram.Bot;
using Console.Advanced;
using Console.Advanced.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register Bot configuration
        services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));

        // Register named HttpClient to benefits from IHttpClientFactory and consume it with ITelegramBotClient typed client.
        // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        // and https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
                    ArgumentNullException.ThrowIfNull(botConfiguration);
                    TelegramBotClientOptions options = new(botConfiguration.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();
