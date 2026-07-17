using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.ValueObjects;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommandHandler(
    IVeiculoGateway veiculoGateway,
    IClienteGateway clienteGateway,
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<CriarOrdemServicoCommand, CriarOrdemServicoCommandResponse>
{
    public async Task<CriarOrdemServicoCommandResponse> Handle(CriarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoGateway.GetByIdAsync(request.IdVeiculo);
        if (veiculo is null)
        {
            throw new DomainNotFoundException($"Veículo com id {request.IdVeiculo} não encontrado");
        }

        var dono = await clienteGateway.GetByIdAsync(veiculo.IdDono);
        if (dono is null)
        {
            throw new DomainNotFoundException($"Cliente com id {veiculo.IdDono} não encontrado");
        }

        var clienteOrdemServico = new ClienteOrdemServico
        {
            Id = dono.Id,
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
        await ordemServicoGateway.CriarAsync(ordemServico);

        await mediator.Publish(new DomainEventNotification<OrdemServicoCriadaEvent>(new OrdemServicoCriadaEvent(ordemServico.Id)), cancellationToken);
        
        return new CriarOrdemServicoCommandResponse
        {
            Id = ordemServico.Id,
            Status = StatusOrdemServico.Recebida,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ClienteOrdemServicoResponse.From(clienteOrdemServico),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemServico.Veiculo),
        };
    }
}