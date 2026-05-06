using Application.Veiculo.Commands;
using Domain.Repositories;
using MediatR;

namespace Application.Veiculo.Queries.Handlers;

public class GetAllVeiculosQueryHandler(IVeiculoRepository veiculoRepository)
    : IRequestHandler<GetAllVeiculosQuery, IEnumerable<VeiculoResponse>>
{
    public async Task<IEnumerable<VeiculoResponse>> Handle(GetAllVeiculosQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await veiculoRepository.GetAllAsync();
        return veiculos.Select(v => new VeiculoResponse
        {
            Id = v.Id,
            Placa = v.Placa,
            DonoId = v.DonoId,
            Modelo = v.Modelo,
            Marca = v.Marca
        }).ToList();
    }
}
