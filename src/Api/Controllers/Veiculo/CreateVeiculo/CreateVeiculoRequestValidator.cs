using System.Text.RegularExpressions;
using Api.Contracts.Validation;

namespace Api.Controllers.Veiculo.CreateVeiculo;

public partial class CreateVeiculoRequestValidator : IValidator<CreateVeiculoRequest>
{
    private static readonly Regex PlacaRegex = MyRegex();

    public ValidationResult Validate(CreateVeiculoRequest request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Placa))
        {
            result.Errors.Add("Placa não pode estar vazia.");
        }
        else if (!PlacaRegex.IsMatch(request.Placa))
        {
            result.Errors.Add("Placa inválida.");
        }

        if (request.IdCliente <= 0)
        {
            result.Errors.Add("IdCliente deve ser um cliente válido.");
        }

        if (string.IsNullOrWhiteSpace(request.Modelo))
        {
            result.Errors.Add("Modelo não pode estar vazio.");
        }

        if (string.IsNullOrWhiteSpace(request.Marca))
        {
            result.Errors.Add("Marca não pode estar vazia.");
        }

        return result;
    }

    [GeneratedRegex(@"^[A-Za-z]{3}-?\d[A-Za-z0-9]\d{2}$")]
    private static partial Regex MyRegex();
}
