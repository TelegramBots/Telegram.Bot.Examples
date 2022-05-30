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
            Function function = new Function();
            TestLambdaContext context = new TestLambdaContext();
            JObject input = new JObject();

            string functionResult = await function.FunctionHandler(request: input, context: context);

            Assert.StartsWith("fine from lambda bot", functionResult);
        }
    }
}
