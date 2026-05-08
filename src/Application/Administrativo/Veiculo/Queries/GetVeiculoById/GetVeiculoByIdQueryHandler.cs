using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.GetVeiculoById;

public class GetVeiculoByIdQueryHandler(IVeiculoRepository veiculoRepository)
    : IRequestHandler<GetVeiculoByIdQuery, VeiculoResponse?>
{
    public async Task<VeiculoResponse?> Handle(GetVeiculoByIdQuery request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoRepository.GetByIdAsync(request.Id);
        if (veiculo == null)
        {
            return null;
        }

        return new VeiculoResponse
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa,
            IdDono = veiculo.IdDono,
            Modelo = veiculo.Modelo,
            Marca = veiculo.Marca
        };
    }
}
