using Microsoft.AspNetCore.Mvc;

namespace HH_2026_WebServer.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Pings the server to check connectivity
        /// </summary>
        /// <returns>Pong</returns>
        [HttpGet]
        [Route("api/test/ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("Pong");
        }
    }
}
