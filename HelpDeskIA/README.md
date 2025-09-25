# HelpDeskIA (MVP)

Projeto mínimo viável de HelpDesk com integração OpenAI (API Key).
Conteúdo gerado automaticamente por ChatGPT para você subir no GitHub.

## Estrutura
- HelpDeskIA.Api/         -> ASP.NET Core Web API (controllers, models, services)
- HelpDeskIA.Worker/      -> Worker skeleton para processamento assíncrono
- sql/init.sql           -> Script inicial do banco de dados (MS SQL)

## Como usar
1. Abra a solução ou os projetos no Visual Studio / VS Code.
2. Atualize `HelpDeskIA.Api/appsettings.json` com sua connection string e `OpenAI:ApiKey`.
3. Rode o script SQL `sql/init.sql` no seu SQL Server para criar as tabelas.
4. No diretório `HelpDeskIA.Api`:
   - `dotnet restore`
   - `dotnet build`
   - `dotnet run`
5. API padrão: `https://localhost:5001` (ou porta definida pela sua máquina).

## Endpoints principais
- POST /api/tickets         -> cria ticket (ex.: {"title":"...","description":"...","createdBy":1})
- GET  /api/tickets/{id}    -> obtém ticket
- POST /api/selfservice/ask -> pergunta ao assistente IA (body: string question)
- POST /api/selfservice/escalate/{sessionId} -> escala sessão para ticket
- POST /api/feedback        -> envia avaliação {ticketId,userId,rating,comment}

## Observações
- O projeto usa placeholders para a API Key do OpenAI. Não insira chaves em repositório público.
- Este é um scaffold (esqueleto) — ajuste autenticação, validações e production settings antes de usar em produção.


## Novas features incluídas

- Autenticação JWT (endpoints /api/account/register e /api/account/login).
- Queue via RabbitMQ e um HostedService que consome jobs de classificação.
- Masking de PII antes de enviar texto à OpenAI.
- Parsing robusto da resposta de classificação (espera JSON).
- Projeto de testes (xUnit) cobrindo Masking e geração de token JWT.
- Workflow GitHub Actions para build e testes.
