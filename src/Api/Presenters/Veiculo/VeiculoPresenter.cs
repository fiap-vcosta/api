using Api.ViewModels.Veiculo;
using Application.UseCases.Administrativo.Veiculo.Responses;

namespace Api.Presenters.Veiculo;

public class VeiculoPresenter
{
    public VeiculoViewModel Present(VeiculoResponse response)
    {
        return new VeiculoViewModel
        {
            Id = response.Id,
            IdDono = response.IdDono,
            Placa = response.Placa,
            Modelo = response.Modelo,
            Marca = response.Marca
        };
    }

    public IEnumerable<VeiculoViewModel> Present(IEnumerable<VeiculoResponse> responses)
    {
        return responses.Select(Present);
    }
}
