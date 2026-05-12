using Infrastructure.Services;

namespace UnitTests.Infrastructure.Services;

public class SMTPServiceTests
{
    [Fact]
    public async Task EnviarEmail_CompletesSuccessfully()
    {
        // Arrange
        var service = new SMTPService();
        var email = "teste@teste.com";
        var conteudo = "Teste de corpo";

        // Act && Assert method completes without exception
        await service.EnviarEmail(email, conteudo);
    }
}
