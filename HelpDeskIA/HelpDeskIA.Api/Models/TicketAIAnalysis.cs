namespace HelpDeskIA.Api.Models {
    public class TicketAIAnalysis {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? Summary { get; set; }
        public string? SuggestedResponse { get; set; }
        public double Confidence { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
