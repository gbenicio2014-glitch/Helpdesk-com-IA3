using System.Threading;
using System.Threading.Tasks;

Console.WriteLine("HelpDeskIA Worker skeleton iniciado.");
Console.WriteLine("Este worker deve ser expandido para processar filas (RabbitMQ/Azure) e chamadas assíncronas à OpenAI.");

while (true) {
    // aqui você ligaria um listener de fila e processaria jobs, por exemplo: classificar tickets pendentes
    Console.WriteLine($"Worker heartbeat: {DateTime.UtcNow}");
    await Task.Delay(TimeSpan.FromSeconds(30));
}
