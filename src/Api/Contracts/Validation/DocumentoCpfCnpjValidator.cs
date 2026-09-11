using CpfCnpjLibrary;
using Domain.Administrativo;

namespace Api.Contracts.Validation;

public static class DocumentoCpfCnpjValidator
{
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

        normalized = DocumentoNormalizer.Normalize(documento);
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
