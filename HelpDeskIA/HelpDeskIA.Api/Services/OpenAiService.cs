using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using HelpDeskIA.Api.Models;

namespace HelpDeskIA.Api.Services {
    public class OpenAiService {
        private readonly OpenAIClient _client;

        public OpenAiService(IConfiguration config) {
            var apiKey = config["OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("OpenAI:ApiKey is not configured in appsettings.");
            _client = new OpenAIClient(apiKey);
        }

        public async Task<ClassificationResult> ClassifyTicketAsync(string title, string description) {
            // Mask PII before sending
            var safeTitle = MaskingService.MaskPII(title ?? "");
            var safeDescription = MaskingService.MaskPII(description ?? "");

            var prompt = $@"
Você é um classificador de tickets. Retorne estritamente um JSON com as chaves:
{{ ""category"": string, ""priority"": ""Low|Medium|High|Critical"", ""summary"": string, ""confidence"": number }}
Use as informações:
Título: {safeTitle}
Descrição: {safeDescription}
";

            var chatOptions = new ChatCompletionsCreateRequest {
                Model = "gpt-4o-mini",
                Messages = new List<ChatMessage> {
                    new ChatMessage(ChatRole.User, prompt)
                },
                Temperature = 0.0m
            };

            var response = await _client.ChatCompletions.CreateAsync(chatOptions);
            var raw = response.Choices[0].Message.Content ?? "";
            // try to extract JSON from the response
            try {
                var json = JsonDocument.Parse(raw);
                var root = json.RootElement;
                return new ClassificationResult {
                    Category = root.GetProperty("category").GetString(),
                    Priority = root.GetProperty("priority").GetString(),
                    Summary = root.GetProperty("summary").GetString(),
                    Confidence = root.GetProperty("confidence").GetDouble()
                };
            } catch {
                // fallback: put raw into summary
                return new ClassificationResult {
                    Category = null,
                    Priority = null,
                    Summary = raw,
                    Confidence = 0.0
                };
            }
        }

        public async Task<string> GetAutoResponseAsync(string question) {
            var safeQuestion = MaskingService.MaskPII(question ?? "");
            var prompt = $@"
Você é um assistente de helpdesk.
Pergunta do usuário: {safeQuestion}
Forneça uma resposta clara e profissional, em no máximo 5 frases.
Se não souber a resposta, diga 'Não encontrei uma solução, abrirei um ticket para você'.
";
            var chatOptions = new ChatCompletionsCreateRequest {
                Model = "gpt-4o-mini",
                Messages = new List<ChatMessage> {
                    new ChatMessage(ChatRole.User, prompt)
                }
            };
            var response = await _client.ChatCompletions.CreateAsync(chatOptions);
            return response.Choices[0].Message.Content ?? "";
        }
    }

    public class ClassificationResult {
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? Summary { get; set; }
        public double Confidence { get; set; }
    }
}
