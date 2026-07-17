using Application.UseCases.Administrativo.Veiculo.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoById;

public class GetVeiculoByIdQueryHandler(IVeiculoGateway veiculoGateway)
    : IRequestHandler<GetVeiculoByIdQuery, VeiculoResponse?>
{
    public async Task<VeiculoResponse?> Handle(GetVeiculoByIdQuery request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoGateway.GetByIdAsync(request.Id);
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
