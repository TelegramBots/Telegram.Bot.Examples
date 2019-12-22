namespace Telegram.Bot.Examples.Echo
{
    public static class Configuration
    {
        public readonly static string BotToken = "{BOT_TOKEN}";

#if USE_PROXY
        public static class Proxy
        {
            public readonly static string Host = "{PROXY_ADDRESS}";
            public readonly static int Port = 8080;
        }
#endif
    }
}
