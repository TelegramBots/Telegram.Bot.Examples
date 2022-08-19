using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Examples.Polling;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        services.AddSingleton<UpdateHandlers>();
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var o = sp.GetService<IOptions<BotConfiguration>>()?.Value;
            TelegramBotClientOptions options = new(o.BotToken);
            return new TelegramBotClient(options);
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
