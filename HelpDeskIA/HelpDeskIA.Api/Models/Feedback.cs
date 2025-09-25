namespace HelpDeskIA.Api.Models {
    public class Feedback {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int? UserId { get; set; }
        public int Rating { get; set; } // 1 a 5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
