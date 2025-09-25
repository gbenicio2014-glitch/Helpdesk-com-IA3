using HelpDeskIA.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskIA.Api.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
        public DbSet<TicketAIAnalysis> TicketAIAnalyses => Set<TicketAIAnalysis>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<SelfServiceSession> SelfServiceSessions => Set<SelfServiceSession>();
    }
}
