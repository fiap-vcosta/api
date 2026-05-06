using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Veiculo.Commands.Handlers;

public class CreateVeiculoCommandHandler(IClienteRepository clienteRepository, IVeiculoRepository veiculoRepository)
    : IRequestHandler<CreateVeiculoCommand, VeiculoResponse>
{
    public async Task<VeiculoResponse> Handle(CreateVeiculoCommand request, CancellationToken cancellationToken)
    {
        var dono = await clienteRepository.GetByIdAsync(request.DonoId);
        if (dono == null)
        {
            throw new KeyNotFoundException("Dono não encontrado.");
        }

        var existingVeiculo = await veiculoRepository.GetByPlacaAsync(request.Placa);
        if (existingVeiculo != null)
        {
            throw new InvalidOperationException("Já existe um veículo com esta placa.");
        }

        var veiculo = new Domain.Entities.Veiculo
        {
            Placa = request.Placa,
            DonoId = request.DonoId,
            Modelo = request.Modelo,
            Marca = request.Marca
        };

        await veiculoRepository.CreateAsync(veiculo);

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
