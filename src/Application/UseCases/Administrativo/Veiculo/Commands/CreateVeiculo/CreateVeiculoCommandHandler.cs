using Application.Abstractions.Gateways;
using Domain.Exceptions;

using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Commands.CreateVeiculo;

public class CreateVeiculoCommandHandler(IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway)
    : IRequestHandler<CreateVeiculoCommand, VeiculoResponse>
{
    public async Task<VeiculoResponse> Handle(CreateVeiculoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByIdAsync(request.IdCliente);
        if (cliente == null)
        {
            throw new DomainNotFoundException("Cliente não encontrado.");
        }

        var existingVeiculo = await veiculoGateway.GetByPlacaAsync(request.Placa);
        if (existingVeiculo != null)
        {
            throw new BusinessRuleException("Já existe um veículo com esta placa.");
        }

        var veiculo = new Domain.Administrativo.Entities.VeiculoAggregateRoot
        {
            Placa = request.Placa,
            IdCliente = request.IdCliente,
            Modelo = request.Modelo,
            Marca = request.Marca
        };

        await veiculoGateway.CreateAsync(veiculo);

        return new VeiculoResponse
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa,
            IdCliente = veiculo.IdCliente,
            Modelo = veiculo.Modelo,
            Marca = veiculo.Marca
        };
    }
}
