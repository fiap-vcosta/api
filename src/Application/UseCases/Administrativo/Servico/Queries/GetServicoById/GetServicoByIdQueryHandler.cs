using Application.UseCases.Administrativo.Servico.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Queries.GetServicoById;

public class GetServicoByIdQueryHandler(IServicoGateway servicoGateway)
    : IRequestHandler<GetServicoByIdQuery, ServicoResponse?>
{
    public async Task<ServicoResponse?> Handle(GetServicoByIdQuery request, CancellationToken cancellationToken)
    {
        var servico = await servicoGateway.GetByIdAsync(request.Id);
        if (servico == null)
        {
            return null;
        }

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
