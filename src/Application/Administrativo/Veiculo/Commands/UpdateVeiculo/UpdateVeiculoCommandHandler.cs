using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Veiculo.Commands.UpdateVeiculo;

public class UpdateVeiculoCommandHandler(IClienteRepository clienteRepository, IVeiculoRepository veiculoRepository)
    : IRequestHandler<UpdateVeiculoCommand, VeiculoResponse>
{
    public async Task<VeiculoResponse> Handle(UpdateVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoRepository.GetByIdAsync(request.Id);
        if (veiculo == null)
        {
            throw new KeyNotFoundException($"Veículo com id {request.Id} não encontrado");
        }

        var dono = await clienteRepository.GetByIdAsync(request.DonoId);
        if (dono == null)
        {
            throw new KeyNotFoundException("Dono não encontrado.");
        }

        var existingVeiculo = await veiculoRepository.GetByPlacaAsync(request.Placa);
        if (existingVeiculo != null && existingVeiculo.Id != veiculo.Id)
        {
            throw new InvalidOperationException("Já existe um veículo com esta placa.");
        }

        veiculo.Placa = request.Placa;
        veiculo.DonoId = request.DonoId;
        veiculo.Modelo = request.Modelo;
        veiculo.Marca = request.Marca;

        await veiculoRepository.UpdateAsync(veiculo);

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
