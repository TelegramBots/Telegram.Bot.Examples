using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LambdaBot.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestFunction()
        {
            Function function = new();
            TestLambdaContext context = new();
            JObject input = new();

            string functionResult = await function.FunctionHandler(request: input, context: context);

            Assert.StartsWith("fine from lambda bot", functionResult, StringComparison.InvariantCulture);
        }
    }
}
