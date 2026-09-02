using Api.ViewModels.Cliente;
using Api.Presenters.Veiculo;
using Application.UseCases.Administrativo.Cliente.Responses;

namespace Api.Presenters.Cliente;

public class ClientePresenter(VeiculoPresenter veiculoPresenter)
{
    public ClienteViewModel Present(ClienteResponse response)
    {
        return new ClienteViewModel
        {
            Id = response.Id,
            Nome = response.Nome,
            TipoDocumento = response.TipoDocumento,
            Documento = response.Documento,
            Veiculos = veiculoPresenter.Present(response.Veiculos).ToList()
        };
    }

    public IEnumerable<ClienteViewModel> Present(IEnumerable<ClienteResponse> responses)
    {
        return responses.Select(Present);
    }
}
