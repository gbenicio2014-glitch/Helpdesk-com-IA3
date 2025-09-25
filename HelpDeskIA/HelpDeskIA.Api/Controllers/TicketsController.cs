using HelpDeskIA.Api.Data;
using HelpDeskIA.Api.Models;
using HelpDeskIA.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskIA.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase {
        private readonly AppDbContext _db;
        private readonly QueueService _queue;

        public TicketsController(AppDbContext db, QueueService queue) {
            _db = db;
            _queue = queue;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket) {
            ticket.TicketNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            // Enqueue job for classification
            _queue.Enqueue(new { Job = "classify_ticket", TicketId = ticket.Id });

            return Ok(ticket);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id) {
            var ticket = await _db.Tickets.Include(t => t.Messages).FirstOrDefaultAsync(t => t.Id == id);
            return ticket == null ? NotFound() : Ok(ticket);
        }
    }
}
