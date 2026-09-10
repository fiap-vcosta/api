using Api.Contracts.Validation;

namespace UnitTests.Api.Contracts.Validation;

public class DocumentoNormalizerTests
{
    [Fact]
    public void Normalize_RemovesFormattingCharacters()
    {
        // Arrange / Act
        var result = DocumentoNormalizer.Normalize("111.444.777-35");

        // Assert
        Assert.Equal("11144477735", result);
    }

    [Fact]
    public void TryNormalizeValidCpfOrCnpj_ReturnsTrue_ForValidCpf()
    {
        // Arrange / Act
        var ok = DocumentoNormalizer.TryNormalizeValidCpfOrCnpj("111.444.777-35", out var normalized, out var errors);

        // Assert
        Assert.True(ok);
        Assert.Equal("11144477735", normalized);
        Assert.Empty(errors);
    }

    [Fact]
    public void TryNormalizeValidCpfOrCnpj_ReturnsFalse_ForInvalidDocumento()
    {
        // Arrange / Act
        var ok = DocumentoNormalizer.TryNormalizeValidCpfOrCnpj("123", out _, out var errors);

        // Assert
        Assert.False(ok);
        Assert.Contains(errors, e => e.Contains("inválido", StringComparison.OrdinalIgnoreCase));
    }
}
