using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.GetAllVeiculos;

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
            IdDono = v.IdDono,
            Modelo = v.Modelo,
            Marca = v.Marca
        }).ToList();
    }
}
