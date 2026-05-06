using Api.Contracts;
using System.Text.RegularExpressions;

namespace Api.Controllers.Servico;

public class UpdateServicoRequestValidator : IValidator<UpdateServicoRequest>
{
    private static readonly Regex CodigoRegex = new(@"^[A-Z]{3}-\d{3}$");

    public ValidationResult Validate(UpdateServicoRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Codigo))
        {
            result.Errors.Add("Código não pode estar vazio.");
        }
        else if (!CodigoRegex.IsMatch(request.Codigo))
        {
            result.Errors.Add("Código inválido. Formato esperado: AAA-123");
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            result.Errors.Add("Nome não pode estar vazio.");
        }

        if (request.PrecoPadrao <= 0)
        {
            result.Errors.Add("Preço padrão deve ser maior que zero.");
        }

        return result;
    }
}
