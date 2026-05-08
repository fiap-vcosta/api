using Domain.Administrativo.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using MediatR;

namespace Application.Core.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommandHandler(
    IVeiculoRepository veiculoRepository,
    IClienteRepository clienteRepository,
    IOrdemServicoRepository ordemServicoRepository
) : IRequestHandler<CriarOrdemServicoCommand, CriarOrdemServicoCommandResponse>
{
    public async Task<CriarOrdemServicoCommandResponse> Handle(CriarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoRepository.GetByIdAsync(request.IdVeiculo);
        if (veiculo is null)
        {
            throw new KeyNotFoundException($"Veículo com id {request.IdVeiculo} não encontrado");
        }

        var dono = await clienteRepository.GetByIdAsync(veiculo.IdDono);
        if (dono is null)
        {
            throw new KeyNotFoundException($"Cliente com id {veiculo.IdDono} não encontrado");
        }

        var clienteOrdemServico = new ClienteOrdemServico
        {
            Nome = dono.Nome,
            Email = dono.Email,
        };

        var veiculoOrdemServico = new VeiculoOrdemServico
        {
            Placa = veiculo.Placa,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
        };

        var ordemServico = OrdemServicoAggregateRoot.Criar(clienteOrdemServico, veiculoOrdemServico);
        await ordemServicoRepository.CriarAsync(ordemServico);
        
        return new CriarOrdemServicoCommandResponse
        {
            Id = ordemServico.Id,
            StatusOrdemServico = StatusOrdemServico.Recebida,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = clienteOrdemServico,
            Veiculo = ordemServico.Veiculo,
        };
    }
}