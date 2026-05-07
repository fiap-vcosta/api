using System.Text.RegularExpressions;
using Api.Contracts.Validation;

namespace Api.Controllers.Servico.UpdateServico;

public partial class UpdateServicoRequestValidator : IValidator<UpdateServicoRequest>
{
    private static readonly Regex CodigoRegex = MyRegex();

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

    [GeneratedRegex(@"^[A-Z]{3}-\d{3}$")]
    private static partial Regex MyRegex();
}
