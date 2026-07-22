using Application.UseCases.Administrativo.Servico.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Queries.GetAllServicos;

public class GetAllServicosQueryHandler(IServicoGateway servicoGateway)
    : IRequestHandler<GetAllServicosQuery, IEnumerable<ServicoResponse>>
{
    public async Task<IEnumerable<ServicoResponse>> Handle(GetAllServicosQuery request, CancellationToken cancellationToken)
    {
        var servicos = await servicoGateway.GetAllAsync();
        return servicos.Select(s => new ServicoResponse
        {
            Id = s.Id,
            Codigo = s.Codigo,
            Nome = s.Nome,
            PrecoPadrao = s.PrecoPadrao,
            Ativo = s.Ativo
        }).ToList();
    }
}
