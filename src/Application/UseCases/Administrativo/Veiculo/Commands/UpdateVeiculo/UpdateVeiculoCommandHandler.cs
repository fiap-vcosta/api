using Application.UseCases.Administrativo.Veiculo.Responses;
using Domain.Exceptions;

using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Commands.UpdateVeiculo;

public class UpdateVeiculoCommandHandler(IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway)
    : IRequestHandler<UpdateVeiculoCommand, VeiculoResponse>
{
    public async Task<VeiculoResponse> Handle(UpdateVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoGateway.GetByIdAsync(request.Id);
        if (veiculo == null)
        {
            throw new DomainNotFoundException($"Veículo com id {request.Id} não encontrado");
        }

        var dono = await clienteGateway.GetByIdAsync(request.IdDono);
        if (dono == null)
        {
            throw new DomainNotFoundException("Dono não encontrado.");
        }

        var existingVeiculo = await veiculoGateway.GetByPlacaAsync(request.Placa);
        if (existingVeiculo != null && existingVeiculo.Id != veiculo.Id)
        {
            throw new BusinessRuleException("Já existe um veículo com esta placa.");
        }

        veiculo.Placa = request.Placa;
        veiculo.IdDono = request.IdDono;
        veiculo.Modelo = request.Modelo;
        veiculo.Marca = request.Marca;

        await veiculoGateway.UpdateAsync(veiculo);

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
