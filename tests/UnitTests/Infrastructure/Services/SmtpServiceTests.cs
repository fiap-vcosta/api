using Infrastructure.Services;

namespace UnitTests.Infrastructure.Services;

public class SmtpServiceTests
{
    [Fact]
    public async Task EnviarEmail_CompletesSuccessfully()
    {
        // Arrange
        var service = new SmtpService();
        var email = "teste@teste.com";
        var conteudo = "Teste de corpo";

        // Act && Assert method completes without exception
        await service.EnviarEmail(email, conteudo);
    }
}
