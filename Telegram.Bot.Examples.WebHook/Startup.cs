using Telegram.Bot.Examples.WebHook.Services;

namespace Telegram.Bot.Examples.WebHook;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
    }

    public IConfiguration Configuration { get; }
    private BotConfiguration BotConfig { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // There are several strategies for completing asynchronous tasks during startup.
        // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
        // We are going to use IHostedService to add and later remove Webhook
        services.AddHostedService<ConfigureWebhook>();

        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.BotToken, httpClient));

        // Dummy business-logic service
        services.AddScoped<HandleUpdateService>();

        // The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
        // incoming webhook updates and send serialized responses back.
        // Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
        //   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#add-newtonsoftjson-based-json-format-support
        services.AddControllers()
                .AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            var token = BotConfig.BotToken;
            endpoints.MapControllerRoute(name: "tgwebhook",
                                         pattern: $"bot/{token}",
                                         new { controller = "Webhook", action = "Post" });
            endpoints.MapControllers();
        });
    }
}
