using Amazon.Lambda.TestUtilities;
using System.Text.Json;
using Telegram.Bot.Serialization;
using Telegram.Bot.Types;
using Xunit;

namespace LambdaBot.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestFunction()
    {
        LambdaFunction function = new();
        TestLambdaContext context = new();
        Update update = new() { Message = new() { Text = "123", Chat = new() { Id = 456 } } };
        var input = JsonSerializer.SerializeToElement(update, JsonSerializerOptionsProvider.Options);

        string functionResult = await function.FunctionHandler(request: input, context: context);

        Assert.StartsWith("fine from lambda bot", functionResult, StringComparison.InvariantCulture);
    }
}
