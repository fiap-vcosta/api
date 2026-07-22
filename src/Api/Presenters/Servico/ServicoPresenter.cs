using Api.ViewModels.Servico;
using Application.UseCases.Administrativo.Servico.Responses;

namespace Api.Presenters.Servico;

public class ServicoPresenter
{
    public ServicoViewModel Present(ServicoResponse response)
    {
        return new ServicoViewModel
        {
            Id = response.Id,
            Codigo = response.Codigo,
            Nome = response.Nome,
            PrecoPadrao = response.PrecoPadrao,
            Ativo = response.Ativo
        };
    }

    public IEnumerable<ServicoViewModel> Present(IEnumerable<ServicoResponse> responses)
    {
        return responses.Select(Present);
    }
}
