using Application.Abstractions.Gateways;
using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;

public class GetVeiculosByClienteQueryHandler(IVeiculoGateway veiculoGateway)
    : IRequestHandler<GetVeiculosByClienteQuery, IEnumerable<VeiculoResponse>>
{
    public async Task<IEnumerable<VeiculoResponse>> Handle(GetVeiculosByClienteQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await veiculoGateway.GetByClienteIdAsync(request.IdCliente);
        return veiculos.Select(v => new VeiculoResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            IdCliente = v.IdCliente,
            Modelo = v.Modelo,
            Marca = v.Marca
        });
    }
}
