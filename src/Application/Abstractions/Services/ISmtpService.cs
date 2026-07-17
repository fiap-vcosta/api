namespace Application.Abstractions.Services;

public interface ISmtpService
{
    public Task EnviarEmail(string email, string conteudo);
}