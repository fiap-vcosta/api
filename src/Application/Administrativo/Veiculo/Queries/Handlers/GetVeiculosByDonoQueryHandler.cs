using Application.Administrativo.Veiculo.Commands;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.Handlers;

public class GetVeiculosByDonoQueryHandler(IVeiculoRepository veiculoRepository)
    : IRequestHandler<GetVeiculosByDonoQuery, IEnumerable<VeiculoResponse>>
{
    public async Task<IEnumerable<VeiculoResponse>> Handle(GetVeiculosByDonoQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await veiculoRepository.GetByDonoIdAsync(request.DonoId);
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
