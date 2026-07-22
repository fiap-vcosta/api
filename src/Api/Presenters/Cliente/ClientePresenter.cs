using Api.ViewModels.Cliente;
using Application.UseCases.Administrativo.Cliente.Responses;

namespace Api.Presenters.Cliente;

public class ClientePresenter
{
    public ClienteViewModel Present(ClienteResponse response)
    {
        return new ClienteViewModel
        {
            Id = response.Id,
            Nome = response.Nome,
            TipoDocumento = response.TipoDocumento,
            Documento = response.Documento
        };
    }

    public IEnumerable<ClienteViewModel> Present(IEnumerable<ClienteResponse> responses)
    {
        return responses.Select(Present);
    }
}
