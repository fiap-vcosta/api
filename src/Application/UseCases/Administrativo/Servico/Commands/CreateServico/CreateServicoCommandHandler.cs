using Application.Abstractions.Gateways;
using Domain.Exceptions;
using Application.UseCases.Administrativo.Servico.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Commands.CreateServico;

public class CreateServicoCommandHandler(IServicoGateway servicoGateway)
    : IRequestHandler<CreateServicoCommand, ServicoResponse>
{
    public async Task<ServicoResponse> Handle(CreateServicoCommand request, CancellationToken cancellationToken)
    {
        var existingServico = await servicoGateway.GetByCodigoAsync(request.Codigo);
        if (existingServico != null)
        {
            throw new BusinessRuleException("Já existe um serviço com este código.");
        }

        var servico = new Domain.Administrativo.Entities.ServicoAggregateRoot
        {
            Codigo = request.Codigo,
            Nome = request.Nome,
            PrecoPadrao = request.PrecoPadrao,
            Ativo = request.Ativo
        };

        await servicoGateway.CreateAsync(servico);

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
