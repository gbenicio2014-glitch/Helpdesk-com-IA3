namespace HelpDeskIA.Api.Models {
    public class User {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "EndUser";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
