using Application.Administrativo.Servico.Commands;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Servico.Queries.Handlers;

public class GetAllServicosQueryHandler(IServicoRepository servicoRepository)
    : IRequestHandler<GetAllServicosQuery, IEnumerable<ServicoResponse>>
{
    public async Task<IEnumerable<ServicoResponse>> Handle(GetAllServicosQuery request, CancellationToken cancellationToken)
    {
        var servicos = await servicoRepository.GetAllAsync();
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
