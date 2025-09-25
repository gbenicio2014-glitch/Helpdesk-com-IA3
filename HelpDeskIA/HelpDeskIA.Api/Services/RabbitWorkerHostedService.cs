using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using HelpDeskIA.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskIA.Api.Services {
    // HostedService running inside API process for demo: listens RabbitMQ and processes classify jobs.
    public class RabbitWorkerHostedService : BackgroundService {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public RabbitWorkerHostedService(IConfiguration config, IServiceProvider services) {
            _config = config;
            _services = services;
            _factory = new ConnectionFactory() {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                UserName = config["RabbitMQ:Username"] ?? "guest",
                Password = config["RabbitMQ:Password"] ?? "guest",
            };
            _queueName = config["RabbitMQ:QueueName"] ?? "helpdesk-jobs";
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) => {
                var body = ea.Body.ToArray();
                var payload = Encoding.UTF8.GetString(body);
                try {
                    var doc = JsonDocument.Parse(payload);
                    if (doc.RootElement.TryGetProperty("Job", out var job) && job.GetString() == "classify_ticket") {
                        var ticketId = doc.RootElement.GetProperty("TicketId").GetInt32();

                        // create scope to use db and AI service
                        using var scope = _services.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var ai = scope.ServiceProvider.GetRequiredService<OpenAiService>();
                        var ticket = await db.Tickets.Include(t => t.Messages).FirstOrDefaultAsync(t => t.Id == ticketId);
                        if (ticket != null) {
                            var result = await ai.ClassifyTicketAsync(ticket.Title, ticket.Description);
                            db.TicketAIAnalyses.Add(new Models.TicketAIAnalysis {
                                TicketId = ticket.Id,
                                Category = result.Category,
                                Priority = result.Priority,
                                Summary = result.Summary,
                                Confidence = result.Confidence
                            });
                            ticket.Category = result.Category ?? ticket.Category;
                            ticket.Priority = result.Priority ?? ticket.Priority;
                            await db.SaveChangesAsync();
                        }
                    }
                } catch {
                    // swallow for demo
                } finally {
                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            return base.StopAsync(cancellationToken);
        }
    }
}
