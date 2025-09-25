using System.Text;
using RabbitMQ.Client;
using System.Text.Json;

namespace HelpDeskIA.Api.Services {
    public class QueueService {
        private readonly IConfiguration _config;
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public QueueService(IConfiguration config) {
            _config = config;
            _factory = new ConnectionFactory() {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                UserName = config["RabbitMQ:Username"] ?? "guest",
                Password = config["RabbitMQ:Password"] ?? "guest",
            };
            _queueName = config["RabbitMQ:QueueName"] ?? "helpdesk-jobs";
        }

        public void Enqueue(object job) {
            using var conn = _factory.CreateConnection();
            using var channel = conn.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
            var payload = JsonSerializer.Serialize(job);
            var body = Encoding.UTF8.GetBytes(payload);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);
        }
    }
}
