using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Examples.DotNetCoreWebHook.Services;
using Telegram.Bot.Types;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        readonly IUpdateService _updateService;
        readonly BotConfiguration _config;

        public UpdateController(IUpdateService updateService, BotConfiguration config)
        {
            _updateService = updateService;
            _config = config;
        }

        // POST api/update
        [HttpPost]
        public void Post([FromBody]Update update)
        {
            _updateService.Echo(update);
        }
    }
}
