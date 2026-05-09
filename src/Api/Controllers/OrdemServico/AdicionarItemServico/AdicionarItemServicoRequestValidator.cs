using Api.Contracts.Validation;

namespace Api.Controllers.OrdemServico.AdicionarItemServico;

public class AdicionarItemServicoRequestValidator : IValidator<AdicionarItemServicoRequest>
{
    public ValidationResult Validate(AdicionarItemServicoRequest request)
    {
    // public class ItemNecessario
    // {
    //     public int IdItemEstoque { get; init; }
    //     public decimal Quantidade { get; init; }
    // }
    //
    // public int IdOrdemServico { get; init; }
    // public int IdServico { get; init; }
    // public decimal ValorCobrado { get; init; }
    // public IList<ItemNecessario> ItensNecessarios { get; init; } = [];
        
        var result = new ValidationResult();

        if (request.IdServico <= 0)
        {
            result.Errors.Add("IdServico deve ser um id válido.");
        }

        if (request.ValorCobrado <= 0)
        {
            result.Errors.Add("ValorCobrado deve ser maior que 0.");
        }

        foreach (var itemNecessario in request.ItensNecessarios)
        {
            if (itemNecessario.IdItemEstoque <= 0)
            {
                result.Errors.Add($"ItensNecessarios.IdItemEstoque deve ser um id válido.");
            }

            if (itemNecessario.Quantidade <= 0)
            {
                result.Errors.Add($"ItensNecessarios.Quantidade deve ser maior que 0.");
            }
        }
        
        return result;
    }
}