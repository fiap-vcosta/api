using Application.UseCases.Administrativo.Servico.Responses;
using Domain.Exceptions;

using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Commands.UpdateServico;

public class UpdateServicoCommandHandler(IServicoGateway servicoGateway)
    : IRequestHandler<UpdateServicoCommand, ServicoResponse>
{
    public async Task<ServicoResponse> Handle(UpdateServicoCommand request, CancellationToken cancellationToken)
    {
        var servico = await servicoGateway.GetByIdAsync(request.Id);
        if (servico == null)
        {
            throw new DomainNotFoundException($"Serviço com id {request.Id} não encontrado");
        }

        var existingServico = await servicoGateway.GetByCodigoAsync(request.Codigo);
        if (existingServico != null && existingServico.Id != servico.Id)
        {
            throw new BusinessRuleException("Já existe um serviço com este código.");
        }

        servico.Codigo = request.Codigo;
        servico.Nome = request.Nome;
        servico.PrecoPadrao = request.PrecoPadrao;
        servico.Ativo = request.Ativo;

        await servicoGateway.UpdateAsync(servico);

        return new ServicoResponse
        {
            Id = servico.Id,
            Codigo = servico.Codigo,
            Nome = servico.Nome,
            PrecoPadrao = servico.PrecoPadrao,
            Ativo = servico.Ativo
        };
    }
}
