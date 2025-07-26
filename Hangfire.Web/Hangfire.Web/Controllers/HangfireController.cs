using HangFireTactics.Web.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private string url = "https://consultwithgriff.com/rss.xml";
        private string directory = $"E:\\Personal\\Learning-Repo\\Hangfire.Web";
        private string filename = "consultwithgriff.json";
        private string tempPath = "";
        public HangfireController(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
            tempPath = Path.Combine(directory, filename);
        }
        [HttpGet("Pull")]
        public IActionResult PullData()
        {
            _backgroundJobClient.Enqueue<WebPuller>(x => x.GetRssItemUrlsAsync(directory, filename));
            return Ok();
        }
    }
}
