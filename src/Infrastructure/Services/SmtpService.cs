using Application.Abstractions.Services;

namespace Infrastructure.Services;

public class SmtpService : ISmtpService
{
    public Task EnviarEmail(string email, string conteudo)
    {
        Console.WriteLine($"[SMTP]<{email}>: {conteudo}");
        return Task.CompletedTask;
    }
}