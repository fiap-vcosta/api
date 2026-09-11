namespace Application.UseCases.OrdemServico;

internal static class OwnershipDocumento
{
    public static string Normalize(string documento) =>
        documento.Replace(".", "").Replace("-", "").Replace("/", "").Trim();

    public static bool Matches(string documentoJwt, string documentoCliente) =>
        string.Equals(Normalize(documentoJwt), Normalize(documentoCliente), StringComparison.Ordinal);
}
