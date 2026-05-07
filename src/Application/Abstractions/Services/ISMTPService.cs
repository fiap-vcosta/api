namespace Application.Abstractions.Services;

public interface ISMTPService
{
    public Task EnviarEmail(string email, string conteudo);
}