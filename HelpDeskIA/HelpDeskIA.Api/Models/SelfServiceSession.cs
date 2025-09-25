namespace HelpDeskIA.Api.Models {
    public class SelfServiceSession {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string? AiResponse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Resolved { get; set; } = false;
        public int? TicketId { get; set; }
    }
}
