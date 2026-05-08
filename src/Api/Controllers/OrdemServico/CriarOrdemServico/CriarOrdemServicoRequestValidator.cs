using Api.Contracts.Validation;

namespace Api.Controllers.OrdemServico.CriarOrdemServico;

public class CriarOrdemServicoRequestValidator : IValidator<CriarOrdemServicoRequest>
{
    public ValidationResult Validate(CriarOrdemServicoRequest request)
    {
        var result = new ValidationResult();

        if (request.IdVeiculo <= 0)
        {
            result.Errors.Add("IdVeiculo deve ser um veículo válido.");
        }
        
        return result;
    }
}