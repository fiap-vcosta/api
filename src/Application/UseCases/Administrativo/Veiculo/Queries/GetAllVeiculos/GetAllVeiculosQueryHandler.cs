using Application.UseCases.Administrativo.Veiculo.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetAllVeiculos;

public class GetAllVeiculosQueryHandler(IVeiculoGateway veiculoGateway)
    : IRequestHandler<GetAllVeiculosQuery, IEnumerable<VeiculoResponse>>
{
    public async Task<IEnumerable<VeiculoResponse>> Handle(GetAllVeiculosQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await veiculoGateway.GetAllAsync();
        return veiculos.Select(v => new VeiculoResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            IdCliente = v.IdCliente,
            Modelo = v.Modelo,
            Marca = v.Marca
        }).ToList();
    }
}
