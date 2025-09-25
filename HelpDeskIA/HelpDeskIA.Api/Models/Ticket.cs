using System.Collections.Generic;
namespace HelpDeskIA.Api.Models {
    public class Ticket {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}
