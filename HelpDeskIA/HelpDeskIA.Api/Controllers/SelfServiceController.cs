using HelpDeskIA.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskIA.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SelfServiceController : ControllerBase {
        private readonly SelfServiceService _selfService;

        public SelfServiceController(SelfServiceService selfService) {
            _selfService = selfService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromQuery] int? userId, [FromBody] string question) {
            var session = await _selfService.AskAI(userId, question);
            return Ok(session);
        }

        [HttpPost("escalate/{sessionId}")]
        public async Task<IActionResult> Escalate(int sessionId) {
            var session = await _selfService.EscalateToTicket(sessionId);
            return Ok(session);
        }
    }
}
