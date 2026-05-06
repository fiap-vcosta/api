using Application.Veiculo.Commands;
using Domain.Repositories;
using MediatR;

namespace Application.Veiculo.Queries.Handlers;

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
            DonoId = veiculo.DonoId,
            Modelo = veiculo.Modelo,
            Marca = veiculo.Marca
        };
    }
}
