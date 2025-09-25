using HelpDeskIA.Api.Data;
using HelpDeskIA.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskIA.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase {
        private readonly AppDbContext _db;

        public FeedbackController(AppDbContext db) {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Feedback feedback) {
            _db.Feedbacks.Add(feedback);
            await _db.SaveChangesAsync();
            return Ok(feedback);
        }

        [HttpGet("ticket/{ticketId}")]
        public IActionResult GetByTicket(int ticketId) {
            var feedbacks = _db.Feedbacks.Where(f => f.TicketId == ticketId).ToList();
            return Ok(feedbacks);
        }
    }
}
