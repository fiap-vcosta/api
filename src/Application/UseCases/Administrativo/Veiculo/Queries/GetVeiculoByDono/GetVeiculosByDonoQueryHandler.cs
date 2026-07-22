using Application.UseCases.Administrativo.Veiculo.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoByDono;

public class GetVeiculosByDonoQueryHandler(IVeiculoGateway veiculoGateway)
    : IRequestHandler<GetVeiculosByDonoQuery, IEnumerable<VeiculoResponse>>
{
    public async Task<IEnumerable<VeiculoResponse>> Handle(GetVeiculosByDonoQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await veiculoGateway.GetByDonoIdAsync(request.IdDono);
        return veiculos.Select(v => new VeiculoResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            IdDono = v.IdDono,
            Modelo = v.Modelo,
            Marca = v.Marca
        }).ToList();
    }
}
