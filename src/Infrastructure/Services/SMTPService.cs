using Application.Abstractions.Services;

namespace Infrastructure.Services;

public class SMTPService : ISMTPService
{
    public Task EnviarEmail(string email, string conteudo)
    {
        Console.WriteLine($"[SMTP]<{email}>: {conteudo}");
        return Task.CompletedTask;
    }
}