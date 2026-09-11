using Domain.Administrativo;

namespace UnitTests.Domain.Administrativo;

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
}
