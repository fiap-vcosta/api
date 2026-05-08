using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.GetVeiculoByDono;

public class GetVeiculosByDonoQueryHandler(IVeiculoRepository veiculoRepository)
    : IRequestHandler<GetVeiculosByDonoQuery, IEnumerable<VeiculoResponse>>
{
    public async Task<IEnumerable<VeiculoResponse>> Handle(GetVeiculosByDonoQuery request, CancellationToken cancellationToken)
    {
        var veiculos = await veiculoRepository.GetByDonoIdAsync(request.IdDono);
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
