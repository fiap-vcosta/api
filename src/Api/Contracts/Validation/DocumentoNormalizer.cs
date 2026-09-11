using CpfCnpjLibrary;
using DomainDocumentoNormalizer = Domain.Administrativo.DocumentoNormalizer;

namespace Api.Contracts.Validation;

public static class DocumentoNormalizer
{
    public static string Normalize(string documento) =>
        DomainDocumentoNormalizer.Normalize(documento);

    public static bool TryNormalizeValidCpfOrCnpj(
        string? documento,
        out string normalized,
        out IReadOnlyList<string> errors)
    {
        var errorList = new List<string>();
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(documento))
        {
            errorList.Add("Documento é obrigatório.");
            errors = errorList;
            return false;
        }

        normalized = Normalize(documento);
        if (!Cpf.Validar(normalized) && !Cnpj.Validar(normalized))
        {
            errorList.Add("Documento inválido.");
            errors = errorList;
            return false;
        }

        errors = errorList;
        return true;
    }
}
