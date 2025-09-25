using HelpDeskIA.Api.Data;
using HelpDeskIA.Api.Models;

namespace HelpDeskIA.Api.Services {
    public class SelfServiceService {
        private readonly AppDbContext _db;
        private readonly OpenAiService _ai;

        public SelfServiceService(AppDbContext db, OpenAiService ai) {
            _db = db;
            _ai = ai;
        }

        public async Task<SelfServiceSession> AskAI(int? userId, string question) {
            var aiResponse = await _ai.GetAutoResponseAsync(question);

            var session = new SelfServiceSession {
                UserId = userId,
                Question = question,
                AiResponse = aiResponse,
                Resolved = !string.IsNullOrWhiteSpace(aiResponse)
            };

            _db.SelfServiceSessions.Add(session);
            await _db.SaveChangesAsync();

            return session;
        }

        public async Task<SelfServiceSession> EscalateToTicket(int sessionId) {
            var session = await _db.SelfServiceSessions.FindAsync(sessionId);
            if (session == null) throw new Exception("Sessão não encontrada");

            var ticket = new Ticket {
                TicketNumber = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                Title = "Escalado de autoatendimento",
                Description = $"{session.Question}\n\nResposta IA: {session.AiResponse}",
                Status = "Open",
                CreatedBy = session.UserId ?? 0
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            session.TicketId = ticket.Id;
            session.Resolved = false;
            await _db.SaveChangesAsync();

            return session;
        }
    }
}
