using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.AdicionarItemOrdemServico;
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

        var cliente = await clienteGateway.GetByIdAsync(veiculo.IdCliente);
        if (cliente is null)
        {
            throw new DomainNotFoundException($"Cliente com id {veiculo.IdCliente} não encontrado");
        }

        var clienteOrdemServico = new ClienteOrdemServico
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
        };

        var veiculoOrdemServico = new VeiculoOrdemServico
        {
            Placa = veiculo.Placa,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
        };

        var ordemServico = OrdemServicoAggregateRoot.Criar(clienteOrdemServico, veiculoOrdemServico);
        await ordemServicoGateway.CriarAsync(ordemServico);

        foreach (var servicoRequest in request.Servicos)
        {
            await mediator.Send(new AdicionarItemOrdemServicoCommand
            {
                IdOrdemServico = ordemServico.Id,
                IdServico = servicoRequest.IdServico,
                ValorCobrado = servicoRequest.ValorCobrado,
                ItensNecessarios = servicoRequest.ItensNecessarios.Select(item =>
                    new AdicionarItemOrdemServicoCommand.ItemNecessario
                    {
                        IdItemEstoque = item.IdItemEstoque,
                        Quantidade = item.Quantidade
                    }).ToList()
            }, cancellationToken);
        }

        await mediator.Publish(new DomainEventNotification<OrdemServicoCriadaEvent>(new OrdemServicoCriadaEvent(ordemServico.Id)), cancellationToken);

        var ordemAtualizada = await ordemServicoGateway.GetByIdAsync(ordemServico.Id)
            ?? throw new DomainNotFoundException($"Ordem de Serviço com id {ordemServico.Id} não encontrada");

        return new CriarOrdemServicoCommandResponse
        {
            Id = ordemAtualizada.Id,
            Status = ordemAtualizada.Status,
            ValorTotal = ordemAtualizada.ValorTotal,
            RecebidaEm = ordemAtualizada.RecebidaEm,
            Cliente = ClienteOrdemServicoResponse.From(ordemAtualizada.Cliente),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemAtualizada.Veiculo),
            Servicos = ServicoOrdemServicoResponse.FromMany(ordemAtualizada.Servicos),
        };
    }
}
